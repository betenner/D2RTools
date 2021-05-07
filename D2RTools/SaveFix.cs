using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using D2SaveFile;

namespace D2RTools
{
    /// <summary>
    /// Fix save file for D2R Technical Alpha
    /// </summary>
    public static class SaveFix
    {
        private const byte GAME_COMPLETED_ON_NORMAL = 0x08;

        private const int CHARACTER_PROGRESSION_OFFSET = 0x25;
        private const int CHECKSUM_OFFSET = 0x0C;

        private const int QUESTS_SECTION_OFFSET = 0x014F;

        private const byte WAYPOINTS_A3WP1_ENABLED = 0x04;
        private const int WAYPOINTS_SECTION_OFFSET = 0x0279;
        private const int WAYPOINTS_DATA_OFFSET = 0x08;
        private const int WAYPOINTS_DIFFICULTY_OFFSET = 0x18;

        /// <summary>
        /// Fix save file
        /// </summary>
        /// <param name="filePath"></param>
        public static void Fix(string filePath)
        {
            // 读取
            var fileData = File.ReadAllBytes(filePath);

            // 解锁
            UnlockGame(fileData);

            // Checksum
            UpdateChecksum(fileData, CHECKSUM_OFFSET);

            // 写入
            File.WriteAllBytes(filePath, fileData);
        }

        private static void EnableA3WP1(byte[] rawSaveFile)
        {
            for (int difficulty = 0; difficulty < 3; difficulty++)
            {
                int firstWpOffset = WAYPOINTS_SECTION_OFFSET +
                    WAYPOINTS_DATA_OFFSET + difficulty * WAYPOINTS_DIFFICULTY_OFFSET;
                rawSaveFile[firstWpOffset + 4] |= WAYPOINTS_A3WP1_ENABLED;
            }
        }

        private static void AllowTravelToNextAct(Difficulty difficulty, Act act, byte[] rawSaveFile)
        {
            if (act != Act.Act4)
            {
                ChangeQuest(difficulty, act, Quest.Quest6, true, rawSaveFile);
            }
            else
            {
                ChangeQuest(difficulty, act, Quest.Quest2, true, rawSaveFile);
            }
        }

        private static void CompleteA2(Difficulty difficulty, byte[] rawSaveFile)
        {
            AllowTravelToNextAct(difficulty, Act.Act2, rawSaveFile);
        }

        private static void CompleteAllA2(byte[] rawSaveFile)
        {
            CompleteA2(Difficulty.Normal, rawSaveFile);
            CompleteA2(Difficulty.Nightmare, rawSaveFile);
            CompleteA2(Difficulty.Hell, rawSaveFile);
        }

        private static void UnlockGame(byte[] rawSaveFile)
        {
            // Complete normal difficulty
            rawSaveFile[CHARACTER_PROGRESSION_OFFSET] |= GAME_COMPLETED_ON_NORMAL;

            // Complete all Act 2
            CompleteAllA2(rawSaveFile);

            // Unlock Act 3 Waypoint 1
            EnableA3WP1(rawSaveFile);
        }

        private static int GetQuestOffset(Difficulty difficulty, Act act, Quest quest)
        {
            int offset = -1;

            if (act != Act.Act4 || quest < Quest.Quest4)
            {
                offset = 12;                    // 10 bytes for the quest header, 2 bytes for the act introduction

                offset += (int)difficulty * 96; // choose to the right difficulty
                offset += (int)act * 16;        // choose to the right act
                offset += (int)quest * 2;       // choose the right quest

                if (act == Act.Act5)
                {
                    offset += 4;                // there are additional bytes in act 4
                }
            }

            return offset;
        }

        private static void ChangeQuest(Difficulty difficulty, Act act, Quest quest, bool complete, byte[] rawSaveFile)
        {
            int offset = QUESTS_SECTION_OFFSET + GetQuestOffset(difficulty, act, quest);

            if (offset == -1)
            {
                return;
            }

            if (complete)
            {
                rawSaveFile[offset] = 0x01;     // Quest complete
                rawSaveFile[offset + 1] = 0x10; // Quest log animation viewed

                if (act == Act.Act5 && quest == Quest.Quest3)
                {
                    // Scroll of resist
                    rawSaveFile[offset] += 0xC0;
                }
            }
            else
            {
                rawSaveFile[offset] = 0;
                rawSaveFile[offset + 1] = 0;
            }

            // Allow travel to the next act.
            // For Act4, the diablo quest is quest2
            if (complete && (quest == Quest.Quest6 || (act == Act.Act4 && quest == Quest.Quest2)))
            {
                if (act != Act.Act4)
                {
                    rawSaveFile[offset + 2] = 1;
                }
                else
                {
                    rawSaveFile[offset + 4] = 1;
                }
            }
        }

        private static void UpdateChecksum(byte[] fileData, int checkSumOffset)
        {
            if (fileData == null || fileData.Length < checkSumOffset + 4) return;

            // Clear out the old checksum
            Array.Clear(fileData, checkSumOffset, 4);

            int[] checksum = new int[4];
            bool carry = false;

            for (int i = 0; i < fileData.Length; ++i)
            {
                int temp = fileData[i] + (carry ? 1 : 0);

                checksum[0] = checksum[0] * 2 + temp;
                checksum[1] *= 2;

                if (checksum[0] > 255)
                {
                    checksum[1] += (checksum[0] - checksum[0] % 256) / 256;
                    checksum[0] %= 256;
                }

                checksum[2] *= 2;

                if (checksum[1] > 255)
                {
                    checksum[2] += (checksum[1] - checksum[1] % 256) / 256;
                    checksum[1] %= 256;
                }

                checksum[3] *= 2;

                if (checksum[2] > 255)
                {
                    checksum[3] += (checksum[2] - checksum[2] % 256) / 256;
                    checksum[2] %= 256;
                }

                if (checksum[3] > 255)
                {
                    checksum[3] %= 256;
                }

                carry = (checksum[3] & 128) != 0;
            }

            for (int i = checkSumOffset; i < checkSumOffset + 4; ++i)
            {
                fileData[i] = (byte)checksum[i - checkSumOffset];
            }
        }
    }
}
