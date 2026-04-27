using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyFootball.Application.Utils
{
    // Add data to resources (and add external text file for addicional names?)

    public class NameGenerator
    {
        protected static StringCollection first_names = new StringCollection();
        protected static StringCollection last_names = new StringCollection();
        protected static StringCollection team_names = new StringCollection();
        protected static StringCollection team_postfix = new StringCollection();
        protected static StringCollection team_prefix = new StringCollection();

        private Random m_random = new Random((int)DateTime.Now.Ticks);


        public NameGenerator()
        {
            loadNamesFromResource(first_names, "FirstNames.txt");
            loadNamesFromResource(last_names, "LastNames.txt");
            loadNamesFromResource(team_names, "TeamNames.txt");
            loadNamesFromResource(team_postfix, "TeamPostfixNames.txt");
            loadNamesFromResource(team_prefix, "TeamPrefixNames.txt");
        }


        protected bool loadNamesFromResource(StringCollection collection, string resourceFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith($"utils.resources.{resourceFileName}", StringComparison.OrdinalIgnoreCase));
            if (resourceName == null)
                return false;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line[0] != '#')
                    {
                        var name = line.Trim();
                        collection.Add(name);
                    }
                }
            }
            return true;
        }

        public int NumberOfFirstNames { get { return first_names.Count; } }
        public int NumberOfLastNames { get { return last_names.Count; } }
        public int NumberOfTeamNames { get { return team_names.Count; } }
        public int NumberOfTeamPostfixes { get { return team_postfix.Count; } }
        public int NumberOfTeamPrefixes { get { return team_prefix.Count; } }

        public string GetName()
        {
            return GetFirstName() + " " + GetLastName();
        }
        public string GetFirstName()
        {
            var first = first_names[m_random.Next(first_names.Count)];
            return first;
        }

        public string GetLastName()
        {
            var last = last_names[m_random.Next(last_names.Count)];
            return last;
        }
        protected string GetTeamName()
        {
            var last = team_names[m_random.Next(team_names.Count)];
            return last;
        }
        protected string GetTeamPostfix()
        {
            var last = team_postfix[m_random.Next(team_postfix.Count)];
            return last;
        }
        protected string GetTeamPrefix()
        {
            var last = team_prefix[m_random.Next(team_prefix.Count)];
            return last;
        }

        public string GetClubName()
        {
            string name = string.Empty;
            double x = m_random.NextDouble();
            if (x > 0.6) name += GetTeamPrefix() + " ";
            name += GetTeamName();
            if (x < 0.4) name += " " + GetTeamPostfix();
            return name.Trim();
        }
    }
}
