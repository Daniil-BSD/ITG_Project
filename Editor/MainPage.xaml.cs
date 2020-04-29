using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Editor.DataModels;
using ITG_Core;
using ITG_Core.Basic.Builders;
using ITG_Core.Bulders;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Editor {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 


    public sealed partial class MainPage : Page {
        public List<Type> PossibleBuilders { get; private set; }
        public ObservableCollection<BuilderModel> BuilderModels { get; private set; }
        public List<string> PossibleBuildersNames
        {
            get {
                List<string> ret = new List<string>(PossibleBuilders.Count);
                ret.AddRange(from Type builder in PossibleBuilders
                             select builder.Name.Replace("Builder", ""));
                return ret;
            }
        }
        public MainPage()
        {
            IEnumerable<Type> possibleBuilders = (
                    from assemblyType in Assembly.LoadFrom("ITG_Core.dll").GetTypes()
                    where typeof(IAlgorithmBuilder).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
                    select assemblyType );
            PossibleBuilders = new List<Type>(possibleBuilders);
            List<IAlgorithmBuilder> temp = new List<IAlgorithmBuilder> {
                new BlurBuilder(),
                new HydraulicErosionBuilder()
            };
            BuilderModels = new ObservableCollection<BuilderModel>();
            foreach ( IAlgorithmBuilder builder in temp ) {
                BuilderModels.Add(new BuilderModel(builder));
            }

            InitializeComponent();
        }

        private void AddBuilder_Click(object sender, RoutedEventArgs e)
        {
            ComboBox buildersComboBox = (ComboBox)FindName("BuildersComboBox");
            Type builderType = PossibleBuilders[buildersComboBox.SelectedIndex];
            BuilderModels.Add(new BuilderModel((IAlgorithmBuilder)Activator.CreateInstance(builderType)));
        }
    }
}
