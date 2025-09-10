using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ignivault.Data.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net.Http.Json;
    using System.Runtime.CompilerServices;
    using System.Text.Json;

    public class VaultItem : INotifyPropertyChanged
    {
        private int _id;
        private string _userId;
        private string _type;
        private string _name;
        private string _encryptedData;
        private string _iv;
        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime? _updatedAt;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string EncryptedData
        {
            get => _encryptedData;
            set => SetProperty(ref _encryptedData, value);
        }

        public string IV
        {
            get => _iv;
            set => SetProperty(ref _iv, value);
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            set => SetProperty(ref _updatedAt, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public enum VaultItemType
        {
            Text,
            File,
            Login
        }
    }

}
