using System;
using System.Collections.Generic;
using System.Text;

namespace SneakingOutWPF.ViewModel
{
    public class SneakingOutField : ViewModelBase
    {
        private String _text;

        /// <summary>
        /// Felirat lekérdezése, vagy beállítása.
        /// </summary>
        public String Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsSecurity()
        {
            if(_text == "1" || _text == "2")
            {
                return true;
            }
            else { return false; }
        }

        public Boolean IsPlayer()
        {
            if (_text == "3")
            {
                return true;
            }
            else { return false; }
        }

        public Boolean IsWall()
        {
            if (_text == "4")
            {
                return true;
            }
            else { return false; }
        }

        public Boolean IsEmpty()
        {
            if (_text == "0")
            {
                return true;
            }
            else { return false; }
        }

        public Boolean IsExit()
        {
            if (_text == "5")
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Vízszintes koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Függőleges koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 Y { get; set; }

        /// <summary>
        /// Sorszám lekérdezése.
        /// </summary>
        public Int32 Number { get; set; }

        /// <summary>
        /// Lépés parancs lekérdezése, vagy beállítása.
        /// </summary>
        public DelegateCommand StepCommand { get; set; }
    }
}
