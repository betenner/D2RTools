using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data.DataFile
{
    /// <summary>
    /// ItemTypes.txt controller.
    /// </summary>
    public class ItemRatio
    {
        private const int UBER_BITS = 1;
        private const int CS_BITS = 0;

        private static ItemRatio _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static ItemRatio Instance
        {
            get
            {
                if (_instance == null) _instance = new ItemRatio();
                return _instance;
            }
        }

        private Dictionary<int, int> _indices;

        public ItemRatio()
        {
            var data = DataController.Instance[DataFileEnum.ItemRatio];
            if (data == null) return;
            _indices = new Dictionary<int, int>();
            for (int i = 0; i < data.RowCount; i++)
            {
                if (data[i, "Version"] != "1") continue;
                int uberBit = DataHelper.ParseInt(data[i, "Uber"]) * 1 << UBER_BITS;
                int csBit = DataHelper.ParseInt(data[i, "Class Specific"]) * 1 << CS_BITS;
                int bits = uberBit + csBit;
                if (!_indices.ContainsKey(bits)) _indices.Add(bits, i);
            }
        }

        /// <summary>
        /// Calculates base drop rate for item quality
        /// </summary>
        /// <param name="quality">Item quality</param>
        /// <param name="dropLevel">Drop level (area level)</param>
        /// <param name="itemLevel">Item level</param>
        /// <param name="min">[Out] Minimum value</param>
        /// <param name="uber">Is the item exceptional/elite</param>
        /// <param name="classSpecific">Is the item class-specific</param>
        /// <returns></returns>
        public int CalcBaseDropBase(ItemQuality quality, int dropLevel, int itemLevel, out int min,
            bool uber = false, bool classSpecific = false)
        {
            min = 0;
            var data = DataController.Instance[DataFileEnum.ItemRatio];
            if (data == null) return 0;
            if (_indices == null) return 0;
            int bits = (uber ? 1 << UBER_BITS : 0) + (classSpecific ? 1 << CS_BITS : 0);
            if (!_indices.ContainsKey(bits)) return 0;
            int index = _indices[bits];
            int value, divisor;
            string key;
            bool applyMin = false;
            switch (quality)
            {
                case ItemQuality.Superior:
                    key = "HiQuality";
                    break;

                case ItemQuality.Magic:
                    key = "Magic";
                    applyMin = true;
                    break;

                case ItemQuality.Rare:
                    key = "Rare";
                    applyMin = true;
                    break;

                case ItemQuality.Set:
                    key = "Set";
                    applyMin = true;
                    break;

                case ItemQuality.Unique:
                    key = "Unique";
                    applyMin = true;
                    break;

                default:
                    key = "Normal";
                    break;
            }
            value = DataHelper.ParseInt(data[index, key]);
            divisor = DataHelper.ParseInt(data[index, key + "Divisor"]);
            if (applyMin) min = DataHelper.ParseInt(data[index, key + "Min"]);
            if (divisor == 0) return 0;
            return value - (dropLevel - itemLevel) / divisor;
        }
    }
}
