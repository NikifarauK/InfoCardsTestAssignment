using Client.Commands;
using Client.Models;
using Client.Services;
using Client.Views;
using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            CardPresentation.CardSelected += OnCardSelected;
            Refresh(null);
        }


        private readonly RestConnectionServise connection =
            new RestConnectionServise("http://localhost:5000/api/card");
        RestConnectionServise Connection => connection;

        private List<CardDto> cardDtos;


        private string title;
        public string Title
        {
            get => title = "Инфокарты";
            set => SetProperty(ref title, value);
        }

        private int windowWith = 800;
        public int TheWindowWidth
        {
            get => windowWith;
            set
            {
                if (SetProperty(ref windowWith, value))
                    Columns = Math.Max(windowWith / 250, 1);
            }
        }

        private int columns = 800 / 250;
        public int Columns
        {
            get { return columns; }
            set
            {
                SetProperty(ref columns, value); 
            }
        }

        private ICommand refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                    refreshCommand = new Command(Refresh);
                return refreshCommand;
            }
        }

        private async void Refresh(object obj)
        {
            var win = new RefreshWindow();
            win.Show();
            var response = await Connection.Get();
            win.Close();
            if (!(response?.IsSuccessStatusCode ?? false))
            {
                HandleError(response);
                return;
            }
            cardDtos = JsonConvert.DeserializeObject<List<CardDto>>(await response.Content.ReadAsStringAsync())
                .OrderByDescending(x => x.Name).ToList();
            Cards = new ObservableCollection<CardPresentation>(cardDtos.Select(x=>ImageConverter.ToCardPresentation(x)));

            SingleSelected = MultipleSelected = false;
        }


        private ObservableCollection<CardPresentation> cards = new ObservableRangeCollection<CardPresentation>();
        public ObservableCollection<CardPresentation> Cards
        { get => cards; set => SetProperty(ref cards, value); }


        private ICommand addCommand;
        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                    addCommand = new Command(AddCard);
                return addCommand;
            }
        }

        private async void AddCard(object obj)
        {
            var context = new DialogViewModel();
            var dial = new DialogWindow
            {
                DataContext = context,
                Title = "Add New Card"
            };
            var dialog = dial.ShowDialog();
            if (dialog ?? false)
            {
                var newDto = context.Dto;
                newDto.Id = cardDtos?.Aggregate(0, (id, x) => Math.Max(id, x.Id)) + 1 ?? 0;
                var cardStr = JsonConvert.SerializeObject(newDto);
                var res = await Connection.Insert(cardStr);
                if (res?.IsSuccessStatusCode ?? false)
                {
                    cardDtos?.Add(newDto);
                    Refresh(null);
                }
                else
                {
                    HandleError(res);
                }
            }
        }

        private ICommand sortCommand;
        public ICommand SortCommand
        {
            get
            {
                if (sortCommand == null)
                    sortCommand = new Command(DoSort);
                return sortCommand;
            }
        }

        private void DoSort(object obj)
        {
            if (Cards == null || cardDtos == null) return;
            var sorted = Cards?.OrderBy(x => x.Name).ToList();
            cardDtos = cardDtos?.OrderBy(x => x.Name).ToList();
            Cards = new ObservableCollection<CardPresentation>(sorted);
            //    .Clear();
            //foreach (var card in sorted)
            //    Cards.Add(card);
        }

        private void OnCardSelected(object sender, EventArgs e)
        {
            var count = Cards.Count(x => x.IsSelected);
            SingleSelected = count == 1;
            MultipleSelected = count >= 1;
        }

        private bool singleSelected;

        public bool SingleSelected
        {
            get { return singleSelected; }
            set { SetProperty(ref singleSelected, value); }
        }

        private bool multipleSelected;

        public bool MultipleSelected
        {
            get { return multipleSelected; }
            set { SetProperty(ref multipleSelected, value); }
        }

        private ICommand updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (updateCommand == null)
                    updateCommand = new Command(Update);
                return updateCommand;
            }
        }

        private async void Update(object obj)
        {
            var context = new DialogViewModel(Cards.First(x => x.IsSelected));
            var dial = new DialogWindow()
            {
                DataContext = context,
                Title = "Update Card"
            };
            var dialog = dial.ShowDialog();
            if (dialog ?? false)
            {
                if (context.Dto != null)
                {
                    var tmp = JsonConvert.SerializeObject(context.Dto);
                    var res = await Connection.Update(context.Card.Id, tmp);
                    if (!res.IsSuccessStatusCode)
                    {
                        HandleError(res);
                        return;
                    }
                }
                else
                {
                    var dto = cardDtos.First(x => x.Id == context.Card.Id);
                    dto.Name = context.Card.Name;
                    var tmp = JsonConvert.SerializeObject(dto);
                    var res = await Connection.Update(context.Card.Id, tmp);
                    if (!res.IsSuccessStatusCode)
                    {
                        HandleError(res);
                        return;
                    }
                }
                Refresh(null);
            }
        }

        private ICommand deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new Command(Delete);
                return deleteCommand;
            }
        }

        private async void Delete(object obj)
        {
            var listToRemove = Cards.Where(x => x.IsSelected);
            foreach (var card in listToRemove)
            {
                var res = await Connection.Delete(card.Id);
                if (!(res?.IsSuccessStatusCode ?? false))
                {
                    HandleError(res);
                    return ;
                }
            }
            Refresh(null);
        }
        private void HandleError(HttpResponseMessage response, [CallerMemberName] string caller = null)
        {
            string message;
            switch (caller)
            {
                case nameof(Delete):
                    message = " Server cannot delete this card";
                    break;
                case nameof(Update):
                    message = " Server cannot update this card";
                    break;
                case nameof(AddCard):
                    message = " Server cannot save this card";
                    break;
                case nameof(Refresh):
                    message = " Cannot reseave data from server";
                    break;
                default:
                    message = " unexpected error";
                    break;
            }

            MessageBox.Show(response?.StatusCode + message, "ok", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
