using Client.Commands;
using Client.Models;
using Client.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModels
{
    internal class DialogViewModel : ViewModelBase
    {
        public DialogViewModel(CardPresentation card)
        {
            Card = card;
            if (Card.Image != null)
            {
                ImageSource = Card.Image;
                Name = Card.Name;
            }
        }
        public DialogViewModel()
        {
            Card = new CardPresentation();
        }

        public CardPresentation Card { get; set; }

        public CardDto Dto { get; private set; } = null;



        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (SetProperty(ref name, value))
                {
                    Card.Name = value;
                }
            }
        }

        private ICommand selectCommand;
        public ICommand SelectCommand
        {
            get
            {
                if (selectCommand == null)
                    selectCommand = new Command(Select);
                return selectCommand;
            }
        }

        private void Select(object obj)
        {
            var openFile = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Images (*.jpeg, *.jpg, *.png, *.bmp)|*.jpeg; *.jpg; *.png; *.bmp|" +
                "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files" +
                " (*.jpg)|*.jpg"
            };
            var res = openFile.ShowDialog();
            if (res ?? false)
            {
                using (var sr = new StreamReader(openFile.FileName))
                {
                    using (var br = new BinaryReader(sr.BaseStream))
                    {
                        var buff = br.ReadBytes((int)sr.BaseStream.Length);
                        using (var ms = new MemoryStream(buff))
                        {
                            var tmp = Convert.ToBase64String(buff);
                            Dto = new CardDto {Id = Card.Id, Name = Name, Base64Image = tmp };
                            Card.Name = Name;
                            Card.Image = ImageConverter.ConvertFrom64Base(tmp);
                            ImageSource = Card.Image;
                        }
                    }
                }
            }
        }

        private Command okCommand;
        public ICommand OkCommand
        {
            get
            {
                if (okCommand == null)
                {
                    okCommand = new Command(Ok);
                }

                return okCommand;
            }
        }

        private void Ok(object commandParameter)
        {
            if (string.IsNullOrEmpty(Card.Name) || Card.Image == null)
            {
                MessageBox.Show("The Card is not complite", "OK", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var win = commandParameter as Window;
            if (win != null)
            {
                Card.Name = Name;
                if (Dto != null)
                {
                    Dto.Id = Card.Id;
                    Dto.Name = Name;
                }
                win.DialogResult = true;
                win.Close();
            }
        }

        private System.Windows.Media.ImageSource imageSource;
        public System.Windows.Media.ImageSource ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, value);
        }

    }
}
