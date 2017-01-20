﻿using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picard.Lib
{
    public class DataMangler
    {
        public int
            DataVersion = 1;
        public List<string>
            MaterialTypes;
        public List<string>
            IgnoreCommodities;
        public IDictionary<string, int>
            VersionAdded;
        public IDictionary<string, string>
            EliteMatsLookup;
        public IDictionary<string, string>
            MaterialTypeLookup;
        public IDictionary<string, IDictionary<string, int>>
            EngineerCostLookup;

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
        public IDictionary<string, int> FilterAndTranslateMats(IDictionary<string, int> deltas, IDictionary<string, int> removed)
        {
            // TODO: This should really be looking at the data just pulled
            // from Inara
            var ret = new Dictionary<string, int>();

            foreach (var mat in deltas)
            {
                if (EliteMatsLookup.ContainsKey(mat.Key.ToLower()))
                {
                    DeltaTools.AddMat(ret, EliteMatsLookup[mat.Key.ToLower()], mat.Value);
                }
                else if (!IgnoreCommodities.Contains(mat.Key))
                {
                    DeltaTools.AddMat(removed, mat.Key, mat.Value);
                }
            }

            return ret;
        }

        private DataMangler()
        {
            MaterialTypes = new List<string>();
            IgnoreCommodities = new List<string>();
            EliteMatsLookup = new Dictionary<string, string>();
            MaterialTypeLookup = new Dictionary<string, string>();
            EngineerCostLookup = new Dictionary<string, IDictionary<string, int>>();
            VersionAdded = new Dictionary<string, int>();

            // Possible Material Types
            MaterialTypes.Add("Materials");
            MaterialTypes.Add("Data");
            MaterialTypes.Add("Commodities");
            
            using (StringReader r = new StringReader(
                Properties.Resources.EliteInaraLookups))
            {
                string line = "";

                int[] begins  = { -1, -1, -1, -1 };
                int[] lengths = { -1, -1, -1 };
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
                        while(i < 3)
                        {
                            begins[i] = prev;
                            next = line.IndexOf(';', next);
                            lengths[i] = next - prev - 1;
                            prev = next;
                            next++;  i++;
                        }

                        begins[i] = prev;
                        
                        continue;
                    }

                    // Data Line
                    if (begins[0] == -1)
                    {
                        throw new Exception("Data Manglement Exception - Lookup Table Corrupted");
                    }

                    name = line.Substring(begins[0], lengths[0]).Trim();
                    type = line.Substring(begins[1], lengths[1]).Trim();
                    journal = line.Substring(begins[2], lengths[2]).Trim();
                    var derp = line.Substring(begins[3]).Trim();
                    version = int.Parse(derp);

                    if (type == "Irrelevant Commodity")
                    {
                        IgnoreCommodities.Add(name);
                    }
                    else
                    {
                        EliteMatsLookup.Add(journal, name);
                        MaterialTypeLookup.Add(name, type);
                        VersionAdded.Add(name, version);
                    }
                }

                DataVersion = VersionAdded.Values.Max();
            }

            // Logs do not reflect the cost of unlocking an engineer,
            // so we look them up
            var mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Felicity Farseer", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Elvira Martuuk", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("The Dweller", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Liz Ryder", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Tod McQuinn", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Zacariah Nemo", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Lei Cheung", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Hera Tani", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Juri Ishmaak", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Selene Jean", mats);

            mats = new Dictionary<string, int>();
            mats.Add("Modular Terminals", -25);
            EngineerCostLookup.Add("Marco Qwent", mats);

            mats = new Dictionary<string, int>();
            mats.Add("Classified Scan Databanks", -50);
            EngineerCostLookup.Add("Ram Tah", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Broo Tarquin", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Colonel Bris Dekker", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Didi Vatermann", mats);

            mats = new Dictionary<string, int>();
            mats.Add("Unknown Fragment", -25);
            EngineerCostLookup.Add("Professor Palin", mats);

            mats = new Dictionary<string, int>();
            EngineerCostLookup.Add("Lori Jameson", mats);

            mats = new Dictionary<string, int>();
            mats.Add("Decoded Emission Data", -50);
            EngineerCostLookup.Add("Tiana Fortune", mats);

            mats = new Dictionary<string, int>();
            mats.Add("Aberrant Shield Pattern", -50);
            EngineerCostLookup.Add("The Sarge", mats);

            mats = new Dictionary<string, int>();
            mats.Add("Bromellite", -50);
            EngineerCostLookup.Add("Bill Turner", mats);
        }

        public IEnumerable<string> GetUpdates(IDictionary<string, int> data, int ver)
        {
            return from version in VersionAdded
                   where version.Value > ver
                   select version.Key;
        }
    }
}
