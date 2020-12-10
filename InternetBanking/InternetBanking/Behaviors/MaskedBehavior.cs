﻿using System.Collections.Generic;

using Xamarin.Forms;

namespace InternetBanking.Behaviors
{
    public class MaskedBehavior : Behavior<Entry>
    {
        private IDictionary<int, char> _positions;

        private string _mask = "";
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                SetPositions();
            }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;

            base.OnDetachingFrom(bindable);
        }

        private void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positions = null;
                return;
            }

            var list = new Dictionary<int, char>();

            for (var i = 0; i < Mask.Length; i++)
            {
                if (Mask[i] != 'X')
                {
                    list.Add(i, Mask[i]);
                }
            }

            _positions = list;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            var text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || _positions == null)
            {
                return;
            }

            if (text.Length > _mask.Length)
            {
                entry.Text = text.Remove(text.Length - 1);
                return;
            }

            foreach (var position in _positions)
            {
                if (text.Length >= position.Key + 1)
                {
                    var value = position.Value.ToString();

                    if (text.Substring(position.Key, 1) != value)
                    {
                        text = text.Insert(position.Key, value);
                    }
                }
            }

            if (entry.Text != text)
            {
                entry.Text = text;
            }
        }
    }
}
