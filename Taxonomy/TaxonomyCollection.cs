using FileHandlingSystem;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using Taxonomy.Types;

namespace Taxonomy
{
    internal class TaxonomyCollection : ITaxonomyCollection
    {
        #region properties and collections 
        /// <summary>
        /// All taxonomy classes can be expanded with additional properties. The CreateCollection needs to be adapted accordingly.
        /// </summary>
        public ObservableCollection<Domain> Domains { get; set; }
        public ObservableCollection<Kingdom> Kingdoms { get; set; }
        public ObservableCollection<Phylum> Phyla { get; set; }
        public ObservableCollection<SubPhylum> SubPhyla { get; set; }
        public ObservableCollection<Class> Classes { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Family> Families { get; set; }
        public ObservableCollection<Genus> Geni { get; set; }
        public ObservableCollection<Species> Species { get; set; }
        #endregion

        public TaxonomyCollection(string filepath)
        {
            Domains = new ObservableCollection<Domain>();
            Kingdoms = new ObservableCollection<Kingdom>();
            Phyla = new ObservableCollection<Phylum>();
            SubPhyla = new ObservableCollection<SubPhylum>();
            Classes = new ObservableCollection<Class>();
            Orders = new ObservableCollection<Order>();
            Families = new ObservableCollection<Family>();
            Geni = new ObservableCollection<Genus>();
            Species = new ObservableCollection<Species>();
            SetTaxonomyCollections(filepath);
        }

        /// <summary>
        /// private function used by WriteCollection to process Generic information
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_object">the type that the data will be written to</param>
        /// <param name="property"> the property of the type that the data will be written to</param>
        /// <param name="value"> the data that will be written to the property</param>
        /// <returns></returns>
        private bool SetProperty<T>(T _object, string property, object value) where T : class
        {
            PropertyInfo _property = _object.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (_property != null && _property.CanWrite)
            {
                _property.SetValue(_object, value, null);
                return true;
            }
            return false;
        }
        /// <summary>
        /// <typeparamref name="T"/> is the target type. Used to write the T data to the collection of T
        /// <typeparamref name="U"/> is the source type from where data will be read. 
        /// </summary>
        /// <param name="filepath">the path where the .dat file is located</param>
        /// <param name="targetcollection"> the collection of T's where the data will be written to</param>
        /// <param name="sourcecollection"> the collection of U's from which data will be read</param>
        /// <param name="sourcedata"> value for reproductionrate in final species</param>
        private void CreateCollection<T, U>(string filepath, ObservableCollection<T> targetcollection, ObservableCollection<U> sourcecollection, string sourcedata) where T : class where U : class
        {
            T datainstance;
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(filepath, 1);
            string property = typeof(U).ToString().Substring(typeof(U).ToString().LastIndexOf(".")).Trim(new char[] { '.' });
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                datainstance = (T)Activator.CreateInstance(typeof(T));
                SetProperty(datainstance, "Name", splitstring[0].Trim(new char[] { '{', '}' }));
                SetProperty(datainstance, property, sourcecollection[Convert.ToInt32(splitstring[1].Trim(new char[] { '}' }), CultureInfo.InvariantCulture) - 1]);
                SetProperty(datainstance, sourcedata, double.Parse(splitstring[2].Trim(new char[] { '}' }), CultureInfo.InvariantCulture));
                targetcollection.Add(datainstance);
            }
        }
        /// <summary>
        /// <typeparamref name="T"/> is the target type used to write the data to the collection of T
        /// <typeparamref name="U"/> is the source type from where data will be read. 
        /// </summary>
        /// <param name="filepath">the path where the .dat file is located</param>
        /// <param name="targetcollection"> the collection of T's where the data will be written to</param>
        /// <param name="sourcecollection"> the collection of U's from which data will be read</param>
        private void CreateCollection<T, U>(string filepath, ObservableCollection<T> targetcollection, ObservableCollection<U> sourcecollection) where T : class where U : class
        {
            T datainstance;
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(filepath, 1);
            string property = typeof(U).ToString().Substring(typeof(U).ToString().LastIndexOf(".")).Trim(new char[] { '.' });
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                datainstance = (T)Activator.CreateInstance(typeof(T));
                SetProperty(datainstance, "Name", splitstring[0].Trim(new char[] { '{', '}' }));
                SetProperty(datainstance, property, sourcecollection[Convert.ToInt32(splitstring[1].Trim(new char[] { '}' }), CultureInfo.InvariantCulture) - 1]);
                targetcollection.Add(datainstance);
            }
        }
        /// <summary>
        /// <typeparamref name="T"/> is the target type used to write the data to the collection of T
        /// </summary>
        /// <param name="filepath">the path where the .dat file is located</param>
        /// <param name="targetcollection"> the collection of T's where the data will be written to</param>
        private void CreateCollection<T>(string filepath, ObservableCollection<T> targetcollection) where T : class, new()
        {
            T datainstance;
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(filepath, 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                datainstance = (T)Activator.CreateInstance(typeof(T));
                SetProperty(datainstance, "Name", splitstring[0].Trim(new char[] { '{', '}' }));
                targetcollection.Add(datainstance);
            }
        }

        private int SetTaxonomyCollections(string filepath)
        {
            /// Create all collections
            CreateCollection(filepath + "domain.dat", Domains);
            CreateCollection(filepath + "kingdom.dat", Kingdoms, Domains);
            CreateCollection(filepath + "phylum.dat", Phyla, Kingdoms);
            CreateCollection(filepath + "subphylum.dat", SubPhyla, Phyla);
            CreateCollection(filepath + "class.dat", Classes, SubPhyla);
            CreateCollection(filepath + "order.dat", Orders, Classes);
            CreateCollection(filepath + "family.dat", Families, Orders);
            CreateCollection(filepath + "genus.dat", Geni, Families);
            CreateCollection(filepath + "species.dat", Species, Geni, "ReproductionRate");
            return 0;
        }
    }
}
