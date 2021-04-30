using System;
using System.Collections.Generic;

namespace D2Data
{

    /// <summary>
    /// 列表字典，在字典功能的基础上增加列表功能，以便顺序遍历
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ListDict<TKey, TValue>
    {
        // 字典功能
        private Dictionary<TKey, int> _keyMap;

        // 列表功能
        private List<TValue> _list;
        private List<TKey> _keyList;

        private Dictionary<int, int> _freeIndexMap = new Dictionary<int, int>();
        private Queue<int> _freeIndexQueue = new Queue<int>();

        public int Count { get; private set; }

        /// <summary>
        /// 创建一个列表字典的实例
        /// </summary>
        public ListDict()
        {
            _keyMap = new Dictionary<TKey, int>();
            _list = new List<TValue>();
            _keyList = new List<TKey>();
        }

        /// <summary>
        /// 创建一个列表字典的实例并指定初始容量
        /// </summary>
        /// <param name="capacity">初始容量</param>
        public ListDict(int capacity)
        {
            _keyMap = new Dictionary<TKey, int>(capacity);
            _list = new List<TValue>();
            _keyList = new List<TKey>();
        }

        /// <summary>
        /// 创建一个列表字典的实例并使用列表字典进行初始化
        /// </summary>
        /// <param name="listDict">初始化列表字典</param>
        public ListDict(ListDict<TKey, TValue> listDict)
        {
            _keyMap = new Dictionary<TKey, int>();
            _list = new List<TValue>();
            _keyList = new List<TKey>();
            for (int i = 0; i < listDict.Count; i++)
            {
                Add(listDict.Key(i), listDict.At(i));
            }
        }

        /// <summary>
        /// 创建一个列表字典的实例并使用字典进行初始化
        /// </summary>
        /// <param name="dict">初始化字典</param>
        public ListDict(Dictionary<TKey, TValue> dict)
        {
            _keyMap = new Dictionary<TKey, int>();
            _list = new List<TValue>();
            _keyList = new List<TKey>();
            if (dict != null)
            {
                Dictionary<TKey, TValue>.Enumerator e = dict.GetEnumerator();
                while (e.MoveNext())
                {
                    Add(e.Current.Key, e.Current.Value);
                }
            }
        }

        private List<TValue> _tmplist = new List<TValue>();
        private List<TKey> _tmpkeyList = new List<TKey>();

        /// <summary>
        /// 获取指定索引处的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public TValue At(int index)
        {
            if (_freeIndexQueue.Count > 0)
            {
                if (index == 0 || (index >= _tmplist.Count))
                {
                    _tmplist.Clear();
                    int maxCount = Count + _freeIndexQueue.Count;
                    for (int i = 0; i < maxCount; i++)
                    {
                        if (!_freeIndexMap.ContainsKey(i))
                        {
                            _tmplist.Add(_list[i]);
                        }
                    }
                }
                return _tmplist[index];
            }
            return _list[index];
        }

        /// <summary>
        /// 获取指定索引的键
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public TKey Key(int index)
        {
            if (_freeIndexQueue.Count > 0)
            {
                if (index == 0 || (index >= _tmpkeyList.Count))
                {
                    _tmpkeyList.Clear();
                    int maxCount = Count + _freeIndexQueue.Count;
                    for (int i = 0; i < maxCount; i++)
                    {
                        if (!_freeIndexMap.ContainsKey(i))
                        {
                            _tmpkeyList.Add(_keyList[i]);
                        }
                    }
                }
                return _tmpkeyList[index];
            }
            return _keyList[index];
        }

        /// <summary>
        /// 获取值列表
        /// </summary>
        /// <returns></returns>
        public List<TValue> ToList()
        {
            if (_freeIndexQueue.Count > 0)
            {
                _tmplist.Clear();
                int maxCount = Count + _freeIndexQueue.Count;
                for (int i = 0; i < maxCount; i++)
                {
                    if (!_freeIndexMap.ContainsKey(i))
                    {
                        _tmplist.Add(_list[i]);
                    }
                }

                return _tmplist;
            }
            return _list;
        }

        /// <summary>
        /// 获取键列表
        /// </summary>
        /// <returns></returns>
        public List<TKey> KeyList()
        {
            if (_freeIndexQueue.Count > 0)
            {
                _tmpkeyList.Clear();
                int maxCount = Count + _freeIndexQueue.Count;
                for (int i = 0; i < maxCount; i++)
                {
                    if (!_freeIndexMap.ContainsKey(i))
                    {
                        _tmpkeyList.Add(_keyList[i]);
                    }
                }
                return _tmpkeyList;
            }
            return _keyList;
        }

        /// <summary>
        /// 获取或设置指定键对应的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue v))
                {
                    return v;
                }
                return default(TValue);
            }
            set
            {
                if (_keyMap.TryGetValue(key, out int index))
                {
                    _list[index] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (_keyMap.ContainsKey(key))
            {
                throw new ArgumentException("重复键: " + key.ToString());
            }

            if (_freeIndexQueue.Count > 0)
            {
                int index = _freeIndexQueue.Dequeue();
                _freeIndexMap.Remove(index);
                _list[index] = value;
                _keyList[index] = key;
                _keyMap.Add(key, index);
            }
            else
            {
                int index = _list.Count;
                _list.Add(value);
                _keyList.Add(key);
                _keyMap.Add(key, index);
            }
            Count++;
        }

        private TKey _defaultKey = default(TKey);
        private TValue _defaultValue = default(TValue);
        
        /// <summary>
        /// 移除指定条目
        /// </summary>
        /// <param name="key">键</param>
        public void Remove(TKey key)
        {
            if (_keyMap.TryGetValue(key, out int index))
            {
                _keyList[index] = _defaultKey;
                _list[index] = _defaultValue;
                _keyMap.Remove(key);
                _freeIndexQueue.Enqueue(index);
                _freeIndexMap.Add(index, index);
                Count--;
            }
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">[输出]值</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            int index;
            if (_keyMap.TryGetValue(key, out index))
            {
                value = _list[index];
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否包含指定键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _keyMap.ContainsKey(key);
        }

        /// <summary>
        /// 清除全部数据
        /// </summary>
        public void Clear()
        {
            _keyMap.Clear();
            _list.Clear();
            _keyList.Clear();
            _freeIndexQueue.Clear();
            _freeIndexMap.Clear();
            Count = 0;
        }
    }

}