using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayinAlive.Options
{
    internal class ModOptionsDropDown : ModOptionsElement
    {
        private readonly Action<string> _selectOptionsDelegate;
        private string _selectedOption;
        private readonly Action<string> _setOption;
        internal bool greyedOut;
        private bool _canClick => !(_parent is ModOptionsCheckbox) || (_parent as ModOptionsCheckbox)._isChecked;
        public ModOptionsDropDown(
            string label,
            int whichOption,
            Action<string> selectOptionDelegate,
            Func<string> getOption,
            Action<string> setOption,
            ModOptionsCheckbox parent = null)
            : base(label, whichOption, parent)
        {
            _selectOptionsDelegate = selectOptionDelegate;
            _setOption = setOption;

            _selectedOption = getOption();
            _selectOptionsDelegate(_selectedOption);
        }
    }
}
