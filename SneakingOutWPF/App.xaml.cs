using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;
using SneakingOutWPF.ViewModel;
using SneakingOutWPF.Persistence;
using SneakingOutWPF.Model;
using SneakingOutWPF.View;
using Microsoft.Win32;
using System.Windows.Input;

namespace SneakingOutWPF
{
    public enum GameLevel { Level1, Level2, Level3 }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Fields

        private SneakingOutGameModel _model;
        private SneakingOutViewModel _viewModel;
        private Window1 _view;
        private DispatcherTimer _timer;
        private Boolean isPaused;
        private GameLevel _gameLevel;

        #endregion

        #region Constructors

        /// <summary>
        /// Alkalmazás példányosítása.
        /// </summary>
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        #endregion

        #region Application event handlers

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // modell létrehozása
            _model = new SneakingOutGameModel(new SneakingOutFileDataAccess());
            _model.GameOver += new EventHandler<SneakingOutEventArgs>(Model_GameOver);

            // nézemodell létrehozása
            _viewModel = new SneakingOutViewModel(_model);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.Level1 += new EventHandler(Level1);
            _viewModel.Level2 += new EventHandler(Level2);
            _viewModel.Level3 += new EventHandler(Level3);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);
            _viewModel.PauseGame += new EventHandler(ViewModel_PauseGame);
            _viewModel.RestartGame += new EventHandler(ViewModel_RestartGame);
            /*_viewModel.UpKey += new EventHandler(UpKey);
            _viewModel.DownKey += new EventHandler(DownKey);
            _viewModel.RightKey += new EventHandler(RightKey);
            _viewModel.LeftKey += new EventHandler(LeftKey);*/

            // nézet létrehozása
            _view = new Window1();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.Show();

            // időzítő létrehozása
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
            //_timer.Start();

            isPaused = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                _model.AdvanceTime();
            }
        }

        #endregion

        #region View event handlers

        /// <summary>
        /// Nézet bezárásának eseménykezelője.
        /// </summary>
        private void View_Closing(object sender, CancelEventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            if (MessageBox.Show("Are you sure you want to exit?", "Sneaking Out the Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást

                if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                    _timer.Start();
            }
        }
        #endregion

        #region ViewModel event handlers

        /// <summary>
        /// Játék betöltésének eseménykezelője.
        /// </summary>
        private async void ViewModel_StartGame(object sender, System.EventArgs e, GameLevel gameLevel, String fileName)
        {
            _model.NewGame();
            _gameLevel = gameLevel;

            try
            {
                // játék betöltése
                await _model.LoadGameAsync(fileName);
         
            }
            catch (SneakingOutDataException)
            {
                MessageBox.Show("Loading failed!", "SneakingOut the game", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _timer.Start();
            isPaused = false;
        }

        /// <summary>
        /// Játék mentésének eseménykezelője.
        /// </summary>
        private async void ViewModel_SaveGame(object sender, EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
                saveFileDialog.Title = "SneakingOut table loading";
                saveFileDialog.Filter = "SneakingOut table|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        // játéktábla mentése
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (SneakingOutDataException)
                    {
                        MessageBox.Show("Saving failed!" + Environment.NewLine + "The path is incorrect or the file can't be written!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Saving failed!", "SneakingOut the Game", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _timer.Start();
        }

        /// <summary>
        /// Játékból való kilépés eseménykezelője.
        /// </summary>
        private void ViewModel_ExitGame(object sender, System.EventArgs e)
        {
            _view.Close(); // ablak bezárása
        }

        private void Level1(object sender, System.EventArgs e)
        {
            ViewModel_StartGame(sender, e, GameLevel.Level1, @"..\..\..\level1.txt");
        }

        private void Level2(object sender, System.EventArgs e)
        {
            ViewModel_StartGame(sender, e, GameLevel.Level2, @"..\..\..\level2.txt");
        }

        private void Level3(object sender, System.EventArgs e)
        {
            ViewModel_StartGame(sender, e, GameLevel.Level3, @"..\..\..\level3.txt");
        }


        /// <summary>
        /// Játék megallitasanak eseménykezelője.
        /// </summary>
        private void ViewModel_PauseGame(object sender, System.EventArgs e)
        {
            if (!isPaused)
            {
                isPaused = true;
                _timer.Stop();
            }
            else if (isPaused)
            {
                isPaused = false;
                _timer.Start();
            }
        }

        private void ViewModel_RestartGame(object sender, System.EventArgs e)
        {
            if (_gameLevel == GameLevel.Level1)
            {
                Level1(sender, e);
            }
            if (_gameLevel == GameLevel.Level2)
            {
                Level2(sender, e);
            }
            if (_gameLevel == GameLevel.Level3)
            {
                Level3(sender, e);
            }
        }

        /*private void UpKey(object sender, System.EventArgs e)
        {
            if(!isPaused)
            {
                _model.PlayerMove(0);
            }
        }

        private void DownKey(object sender, System.EventArgs e)
        {
            if (!isPaused)
            {
                _model.PlayerMove(1);
            }
        }

        private void RightKey(object sender, System.EventArgs e)
        {
            if (!isPaused)
            {
                _model.PlayerMove(2);
            }
        }

        private void LeftKey(object sender, System.EventArgs e)
        {
            if (!isPaused)
            {
                _model.PlayerMove(3);
            }
        }*/

        #endregion

        #region Model event handlers

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver(object sender, SneakingOutEventArgs e)
        {
            _timer.Stop();

            if (e.IsWon) // győzelemtől függő üzenet megjelenítése
            {
                MessageBox.Show("Congratulations, you won" + Environment.NewLine +
                                "Number of steps" + e.GameStepCount +
                                "The time you needed was: " + TimeSpan.FromSeconds(e.GameTime).ToString("g"),
                                "SneakingOut the Game",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("Nice try, maybe next time!",
                                "SneakingOut the Game",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
        }
        #endregion
    }
}
