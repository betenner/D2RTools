﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2SaveFile.Interfaces
{
    public interface IItemList
    {
        ushort NumberOfItems { get; }
        List<Diablo2Item> GetItems();
        void Add(Diablo2Item item);
    }
}
