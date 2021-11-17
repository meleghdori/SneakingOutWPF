using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using SneakingOutWPF.ViewModel;
using SneakingOutWPF.View;
using Snek

namespace SneakingOutWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Fields

        private SneakingOutGameModel _model;
        private SneakingOutViewModel _viewModel;
        private MainWindow _view;
        private DispatcherTimer _timer;

        #endregion

    }
}
