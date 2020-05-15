using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace BeeBurn
{
    public static class BeeBurnIO
    {
        public static bool LoadImagesToStack(BeeStack stack)
        {
            var dlg = new OpenFileDialog
            {
                InitialDirectory = BeeBurnVM.Get().GetConfigString(ConfigKey.ImageLoadPath),
                Multiselect = true,
                Filter = "Image Files(*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                foreach (string filepath in dlg.FileNames)
                {
                    try
                    {
                        BeeImage bi = new BeeImage(filepath);
                        stack.ActiveImages.Add(bi);
                    }
                    catch (Exception)
                    {
                        // TODO: Handle better.
                        continue;
                    }
                }
                return true;
            }

            return false;
        }

        public static bool SaveAsStack(BeeStack stack)
        {
            string filespec = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var dlg = new SaveFileDialog
            {
                DefaultExt = ".bstack",
                InitialDirectory = BeeBurnVM.Get().GetConfigString(ConfigKey.SavePath),
                Filter = "BStacks (*.bstack)|*.bstack",
                FileName = filespec + ".bstack"
            };

            if (dlg.ShowDialog() == true)
            {
                string savePath = System.IO.Path.GetDirectoryName(dlg.FileName);
                string fileNameNaked = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                string fileExt = System.IO.Path.GetExtension(dlg.FileName);
                if (fileExt.Length < 1)
                {
                    dlg.FileName += ".bstack";
                }

                return stack.SaveStack(fileNameNaked, savePath);
            }

            return false;
        }

        public static string SerializeDictionary(Dictionary<string, string> dict, char sep = '|', char assign = ':')
        {
            string ret = "";
            
            foreach (var kvp in dict)
            {
                if (ret.Length > 0)
                    ret += sep;
                ret += kvp.Key + ":" + kvp.Value;
            }

            return ret;
        }

        public static Dictionary<string, string> DeserializeDictionary(string str, string[] sep, char assign = ':')
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach(string skv in str.Split(sep, System.StringSplitOptions.RemoveEmptyEntries))
            {
                string[] kv = skv.Split(new char[] { assign });
                dict[kv[0].Trim()] = kv[1].Trim();
            }

            return dict;
        }
    }
}
