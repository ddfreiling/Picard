﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using LibEDJournal.State;

namespace LibEDJournal
{
    public class DataMangler
    {
        public int
            DataVersion = 1;
        public List<string>
            MaterialTypes;
        public List<string>
            MaterialOrder;
        public List<string>
            IgnoreCommodities;
        public IDictionary<string, int>
            VersionAdded;
        public IDictionary<string, string>
            EliteMatsLookup;
        public IDictionary<string, string>
            MaterialTypeLookup;
        public IDictionary<string, InventorySet>
            EngineerCostLookup;

        private string IgnoreFile;

        private static DataMangler INSTANCE = null;

        public static DataMangler GetInstance()
        {
            if(INSTANCE == null)
            {
                INSTANCE = new DataMangler();
            }

            return INSTANCE;
        }

        /// <summary>
        /// Filters only known material names in EliteMatsLookup
        /// and also translates them into a localized material name
        /// </summary>
        /// <param name="deltas">The dict to filter and translate</param>
        /// <param name="removed">Will add removed materials to this
        /// dictionary if they are not in the IgnoreCommdoties list</param>
        /// <returns>The filtered and translated dict</returns>
        public InventorySet FilterAndTranslateMats(InventorySet deltas, InventorySet removed)
        {
            // TODO: This should really be looking at the data just pulled
            // from Inara
            var ret = new InventorySet();

            foreach (var mat in deltas)
            {
                if (EliteMatsLookup.ContainsKey(mat.Key.ToLower()))
                {
                    ret.AddMat(EliteMatsLookup[mat.Key.ToLower()], mat.Value);
                }
                else if (!IgnoreCommodities.Contains(mat.Key.ToLower()))
                {
                    removed.AddMat(mat.Key.ToLower(), mat.Value);
                }
            }

            return ret;
        }

        private DataMangler()
        {
            MaterialTypes = new List<string>();
            MaterialOrder = new List<string>();
            IgnoreCommodities = new List<string>();
            EliteMatsLookup = new Dictionary<string, string>();
            MaterialTypeLookup = new Dictionary<string, string>();
            EngineerCostLookup = new Dictionary<string, InventorySet>();
            VersionAdded = new Dictionary<string, int>();

            // Possible Material Types
            MaterialTypes.Add("Materials");
            MaterialTypes.Add("Data");
            MaterialTypes.Add("Commodities");

            // Load from Confirmed Types
            using (StringReader r = new StringReader(
                Properties.Resources.EliteInaraLookups))
            {
                // Current Line will be stored here
                string line = "";

                // Beginnings of columns, marked by ; character
                int[] begins = { -1, -1, -1, -1 };

                // Lengths of columns
                int[] lengths = { -1, -1, -1 };

                // Column Values
                string name, type, journal;
                int version;

                while ((line = r.ReadLine()) != null)
                {
                    // Comment Line
                    if (line[0] == '#' || line.Trim().Length == 0)
                        continue;

                    // Heading Line
                    if (line[0] == ';')
                    {
                        int i = 0, prev = 0, next = 1;
                        while (i < 3)
                        {
                            begins[i] = prev;
                            next = line.IndexOf(';', next);
                            lengths[i] = next - prev - 1;
                            prev = next;
                            next++; i++;
                        }

                        begins[i] = prev;

                        continue;
                    }

                    // Data Line
                    if (begins[0] == -1)
                    {
                        throw new Exception("Data Manglement Exception - Lookup Table Corrupted");
                    }

                    // Parse the Column Values
                    name = line.Substring(begins[0], lengths[0]).Trim();
                    type = line.Substring(begins[1], lengths[1]).Trim();
                    journal = line.Substring(begins[2], lengths[2]).Trim();
                    version = int.Parse(line.Substring(begins[3]).Trim());

                    // Store the Column Values
                    if (type == "Irrelevant Commodity")
                    {
                        IgnoreCommodities.Add(name);
                    }
                    else
                    {
                        MaterialOrder.Add(name);
                        EliteMatsLookup.Add(journal, name);
                        MaterialTypeLookup.Add(name, type);
                        VersionAdded.Add(name, version);
                    }
                }
                
                DataVersion = VersionAdded.Values.Max();
            }

            // Load from Local Ignore File
            string local = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
            string path = @"TickleSoft";
            string file = @"ignore";
            IgnoreFile = Path.Combine(local, path, file);
            if (File.Exists(IgnoreFile)) {
                string[] ignores = File.ReadAllLines(IgnoreFile);
                foreach(var i in ignores)
                {
                    if(!IgnoreCommodities.Contains(i))
                    {
                        IgnoreCommodities.Add(i);
                    }
                }
            }

            InventorySet mats;
            // Logs do not reflect the cost of unlocking an engineer,
            // so we look them up
            mats = new InventorySet();
            EngineerCostLookup.Add("Felicity Farseer", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Elvira Martuuk", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("The Dweller", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Liz Ryder", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Tod McQuinn", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Zacariah Nemo", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Lei Cheung", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Hera Tani", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Juri Ishmaak", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Selene Jean", mats);

            mats = new InventorySet();
            mats.AddMat("modularterminals", -25);
            EngineerCostLookup.Add("Marco Qwent", mats);

            mats = new InventorySet();
            mats.AddMat("scandatabanks", -50);
            EngineerCostLookup.Add("Ram Tah", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Broo Tarquin", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Colonel Bris Dekker", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Didi Vatermann", mats);

            mats = new InventorySet();
            mats.AddMat("unknownenergysource", -25);
            EngineerCostLookup.Add("Professor Palin", mats);

            mats = new InventorySet();
            EngineerCostLookup.Add("Lori Jameson", mats);

            mats = new InventorySet();
            mats.AddMat("decodedemissiondata", -50);
            EngineerCostLookup.Add("Tiana Fortune", mats);

            mats = new InventorySet();
            mats.AddMat("shieldpatternanalysis", -50);
            EngineerCostLookup.Add("The Sarge", mats);

            mats = new InventorySet();
            mats.AddMat("bromellite", -50);
            EngineerCostLookup.Add("Bill Turner", mats);
        }

        public void AddIgnore(InventorySet inv)
        {
            using (StreamWriter w = File.AppendText(IgnoreFile))
            {
                foreach (var i in inv.Keys)
                {
                    if (!IgnoreCommodities.Contains(i))
                    {
                        IgnoreCommodities.Add(i);
                    }

                    w.WriteLine(i);
                }
            }
        }

        public IEnumerable<string> GetUpdates(InventorySet data, int ver)
        {
            return from version in VersionAdded
                   where version.Value > ver
                   select version.Key;
        }
    }
}
