using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using SneakingOutWPF.Model;

namespace SneakingOutWPF.ViewModel
{
    public class SneakingOutViewModel : ViewModelBase
    {
        #region Fields

        private SneakingOutGameModel _model; // modell

        #endregion

        #region Properties

        /// <summary>
        /// Új játék kezdése parancs lekérdezése.
        /// </summary>
        public DelegateCommand NewGameCommand { get; private set; }

        /// <summary>
        /// Játék mentése parancs lekérdezése.
        /// </summary>
        public DelegateCommand SaveGameCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand ExitCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand RestartCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand PauseCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand Level1Command { get; private set; }

        public DelegateCommand Level2Command { get; private set; }

        public DelegateCommand Level3Command { get; private set; }

        public DelegateCommand LeftKeyDownCommand { get;private set; }
        public DelegateCommand RightKeyDownCommand { get;private set; }
        public DelegateCommand DownKeyDownCommand { get;private set; }
        public DelegateCommand UpKeyDownCommand { get;private set; }


        /// <summary>
        /// Játékmező gyűjtemény lekérdezése.
        /// </summary>
        public ObservableCollection<SneakingOutField> Fields { get; set; }

        /// <summary>
        /// Lépések számának lekérdezése.
        /// </summary>
        public Int32 GameStepCount { get { return _model.GameStepCount; } }

        /// <summary>
        /// Fennmaradt játékidő lekérdezése.
        /// </summary>
        public String GameTime { get { return TimeSpan.FromSeconds(_model.GameTime).ToString("g"); } }

        #endregion

        #region Events

        /// <summary>
        /// Játék mentésének eseménye.
        /// </summary>
        public event EventHandler SaveGame;

        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler ExitGame;

        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler Level1;
        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler Level2;
        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler Level3;

        /// <summary>
        /// Játékból megállítása
        /// </summary>
        public event EventHandler PauseGame;

        /// <summary>
        /// Játékból ujrainditasa
        /// </summary>
        public event EventHandler RestartGame;

        public event EventHandler UpKeyDown;
        public event EventHandler DownKeyDown;
        public event EventHandler RightKeyDown;
        public event EventHandler LeftKeyDown;


        #endregion

        #region Constructors

        /// <summary>
        /// Sudoku nézetmodell példányosítása.
        /// </summary>
        /// <param name="model">A modell típusa.</param>
        public SneakingOutViewModel(SneakingOutGameModel model)
        {
            // játék csatlakoztatása
            _model = model;
            _model.GameAdvanced += new EventHandler<SneakingOutEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<SneakingOutEventArgs>(Model_GameOver);
            _model.GameCreated += new EventHandler<SneakingOutEventArgs>(Model_GameCreated);

            // parancsok kezelése
            RestartCommand = new DelegateCommand(param => OnRestartGame());
            PauseCommand = new DelegateCommand(param => OnPauseGame());
            Level1Command = new DelegateCommand(param => OnLevel1());
            Level2Command = new DelegateCommand(param => OnLevel2());
            Level3Command = new DelegateCommand(param => OnLevel3());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());
            UpKeyDownCommand = new DelegateCommand(param => OnUpKeyDown());
            DownKeyDownCommand = new DelegateCommand(param => OnDownKeyDown());
            LeftKeyDownCommand = new DelegateCommand(param => OnLeftKeyDown());
            RightKeyDownCommand = new DelegateCommand(param => OnRightKeyDown());


            // játéktábla létrehozása
            Fields = new ObservableCollection<SneakingOutField>();
            for (Int32 i = 0; i < _model.Table.Size; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < _model.Table.Size; j++)
                {
                    Fields.Add(new SneakingOutField
                    {
                        IsEmpty = false,
                        IsExit = false,
                        IsPlayer = false,
                        IsSecurity = false,
                        IsWall = false,
                        Text = String.Empty,
                        X = i,
                        Y = j,
                        Number = i * _model.Table.Size + j, // a mezo sorszáma, amelyet felhasználunk az azonosításhoz
                        StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                        // ha egy mezőre léptek, akkor jelezzük a léptetést, változtatjuk a lépésszámot
                    });
                }
            }

            RefreshTable();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Tábla frissítése.
        /// </summary>
        public void RefreshTable()
        {
            foreach (SneakingOutField field in Fields) // inicializálni kell a mezőket is
            {
                field.IsEmpty = false;
                field.IsExit = false;
                field.IsPlayer = false;
                field.IsSecurity = false;
                field.IsWall = false;

                if (_model.Table[field.X, field.Y] == 0)
                {
                    field.IsEmpty = true;
                }
                else if (_model.Table[field.X, field.Y] == 1 || _model.Table[field.X, field.Y] == 2)
                {
                    field.IsSecurity = true;
                }
                else if (_model.Table[field.X, field.Y] == 3)
                {
                    field.IsPlayer = true;
                }
                else if (_model.Table[field.X, field.Y] == 4)
                {
                    field.IsWall = true;
                }
                else if (_model.Table[field.X, field.Y] == 5)
                {
                    field.IsExit = true;
                }
                //field.Text = !_model.Table.IsEmpty(field.X, field.Y) ? _model.Table[field.X, field.Y].ToString() : String.Empty;
            }

            OnPropertyChanged("GameTime");
            OnPropertyChanged("GameStepCount");
        }

        /// <summary>
        /// Játék léptetése eseménykiváltása.
        /// </summary>
        /// <param name="index">A lépett mező indexe.</param>
        private void StepGame(Int32 index)
        {
            SneakingOutField field = Fields[index];

            field.Text = _model.Table[field.X, field.Y] > 0 ? _model.Table[field.X, field.Y].ToString() : String.Empty; // visszaírjuk a szöveget
            OnPropertyChanged("GameStepCount"); // jelezzük a lépésszám változást

            field.Text = !_model.Table.IsEmpty(field.X, field.Y) ? _model.Table[field.X, field.Y].ToString() : String.Empty;
        }

        #endregion

        #region Game event handlers

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver(object sender, SneakingOutEventArgs e)
        {

        }

        /// <summary>
        /// Játék előrehaladásának eseménykezelője.
        /// </summary>
        private void Model_GameAdvanced(object sender, SneakingOutEventArgs e)
        {
            OnPropertyChanged("GameTime");
            RefreshTable();
        }

        /// <summary>
        /// Játék létrehozásának eseménykezelője.
        /// </summary>
        private void Model_GameCreated(object sender, SneakingOutEventArgs e)
        {
            RefreshTable();
        }

        #endregion

        #region Event methods

        /// <summary>
        /// jatek megallitasa
        /// </summary>
        private void OnPauseGame()
        {
            if (PauseGame != null)
                PauseGame(this, EventArgs.Empty);
        }

        /// <summary>
        /// jatek ujrakezdese
        /// </summary>
        private void OnRestartGame()
        {
            if (RestartGame != null)
                RestartGame(this, EventArgs.Empty);
        }

        /// <summary>
        /// egyes szint
        /// </summary>
        private void OnLevel1()
        {
            if (Level1 != null)
                Level1(this, EventArgs.Empty);
        }

        /// <summary>
        /// kettes szint
        /// </summary>
        private void OnLevel2()
        {
            if (Level2 != null)
                Level2(this, EventArgs.Empty);
        }

        /// <summary>
        /// harmas szint
        /// </summary>
        private void OnLevel3()
        {
            if (Level3 != null)
                Level3(this, EventArgs.Empty);
        }

        /// <summary>
        /// Játék mentése eseménykiváltása.
        /// </summary>
        private void OnSaveGame()
        {
            if (SaveGame != null)
                SaveGame(this, EventArgs.Empty);
        }

        /// <summary>
        /// Játékból való kilépés eseménykiváltása.
        /// </summary>
        private void OnExitGame()
        {
            if (ExitGame != null)
                ExitGame(this, EventArgs.Empty);
        }

        private void OnUpKeyDown()
        {
            if (UpKeyDown != null)
                UpKeyDown(this, EventArgs.Empty);
        }

        private void OnDownKeyDown()
        {
            if (DownKeyDown != null)
                DownKeyDown(this, EventArgs.Empty);
        }

        private void OnRightKeyDown()
        {
            if (RightKeyDown != null)
                RightKeyDown(this, EventArgs.Empty);
        }

        private void OnLeftKeyDown()
        {
            if(LeftKeyDown != null)
                LeftKeyDown(this, EventArgs.Empty);
        }

        #endregion


    }
}
