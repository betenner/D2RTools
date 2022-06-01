using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data
{
    /// <summary>
    /// Represents a weight list that each element has its own weight value.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    public class WeightList<T>
    {
        #region Fields

        private List<KeyValuePair<T, uint>> _list = new List<KeyValuePair<T, uint>>();

        /// <summary>
        /// Total weight of all elements.
        /// </summary>
        public uint TotalWeight { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an element and its weight into the weight list.
        /// </summary>
        public void Add(T element, uint weight)
        {
            _list.Add(new KeyValuePair<T, uint>(element, weight));
            TotalWeight += weight;
        }

        /// <summary>
        /// Removes specified element.
        /// </summary>
        /// <param name="index">Index of element.</param>
        public void Remove(int index)
        {
            TotalWeight -= _list[index].Value;
            _list.RemoveAt(index);
        }

        /// <summary>
        /// Gets random element based on their weights.
        /// </summary>
        /// <returns></returns>
        public T GetRandomElement()
        {
            return GetRandomElement(false);
        }

        /// <summary>
        /// Gets random element based on their weights.
        /// </summary>
        /// <param name="removeFromList">If true, the randomly chosen element will be removed from the list.</param>
        /// <returns></returns>
        public T GetRandomElement(bool removeFromList)
        {
            uint sum = 0;

            Random rnd = new Random();
            uint weight = (uint)(rnd.NextDouble() * (TotalWeight + 1));

            for (int i = 0; i < _list.Count; i++)
            {
                sum += _list[i].Value;
                if (weight <= sum)
                {
                    T result = _list[i].Key;
                    if (removeFromList) this.Remove(i);
                    return result;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Gets random element based on their weights.
        /// </summary>
        /// <param name="randomSeed">Seed of random generator.</param>
        /// <returns></returns>
        public T GetRandomElement(int randomSeed)
        {
            return GetRandomElement(randomSeed, false);
        }

        /// <summary>
        /// Gets random element based on their weights.
        /// </summary>
        /// <param name="randomSeed">Seed of random generator.</param>
        /// <param name="removeFromlist">If true, the randomly chosen element will be removed from the list.</param>
        /// <returns></returns>
        public T GetRandomElement(int randomSeed, bool removeFromlist)
        {
            uint sum = 0;

            Random rnd = new Random(randomSeed);
            uint weight = (uint)(rnd.NextDouble() * (TotalWeight + 1));

            for (int i = 0; i < _list.Count; i++)
            {
                sum += _list[i].Value;
                if (weight <= sum)
                {
                    T result = _list[i].Key;
                    if (removeFromlist) this.Remove(i);
                    return result;
                }
            }

            return default(T);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the count of elements in this weight list.
        /// </summary>
        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        /// <summary>
        /// Gets list of elements
        /// </summary>
        public List<KeyValuePair<T, uint>> Elements
        {
            get => _list;
        }

        #endregion
    }
}
