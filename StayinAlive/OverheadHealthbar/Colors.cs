using System.ComponentModel.DataAnnotations;

namespace StayinAlive.OverheadHealthbar
{
    public enum Colors : uint
    {
        [Display(Name ="Red")]
        Red = 4281746420,

        [Display(Name = "Pink")]
        Pink = 4284686056,

        [Display(Name = "Purple")]
        Purple = 4289734556,

        [Display(Name = "Deep Purple")]
        DeepPurple = 4290198119,

        [Display(Name = "Indigo")]
        Indigo = 4290072895,

        [Display(Name = "Blue")]
        Blue = 4294153761,

        [Display(Name = "Light Blue")]
        LightBlue = 4294224131,

        [Display(Name = "Cyan")]
        Cyan = 4292131840,

        [Display(Name = "Teal")]
        Teal = 4287141376,

        [Display(Name = "Green")]
        Green = 4283477836,

        [Display(Name = "Light Green")]
        LightGreen = 4283089803,

        [Display(Name = "Lime")]
        Lime = 4281982157,

        [Display(Name = "Yellow")]
        Yellow = 4282117119,

        [Display(Name = "Amber")]
        Amber = 4278698495,

        [Display(Name = "Orange")]
        Orange = 4278229247,

        [Display(Name = "Deep Orange")]
        DeepOrange = 4280440831,

        [Display(Name = "Black")]
        Black = 4278190080u,

        [Display(Name = "White")]
        White = uint.MaxValue,
        // This is here to avoid exceptions if the save file has "Crimson" still.
        Crimson = 4281746420,

    }
}
