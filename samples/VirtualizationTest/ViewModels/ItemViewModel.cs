﻿// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace VirtualizationTest.ViewModels
{
    internal class ItemViewModel
    {
        private int _index;

        public ItemViewModel(int index)
        {
            _index = index;
        }

        public string Header => $"Item {_index}";
    }
}