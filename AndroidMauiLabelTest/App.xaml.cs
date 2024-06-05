#if ANDROID
using Android.Text;
using Android.Views;
using Microsoft.Maui.Controls.Platform;

#endif

using System.Diagnostics;
using System.Numerics;

namespace AndroidMauiLabelTest {
    public partial class App : Application {
        public App() {

            ContentPage mainPage = new();
            this.MainPage = mainPage;

            AbsoluteLayout abs = new();
            mainPage.Content = abs;

            Label label = new();
            label.Margin = new Thickness(0);
            label.Padding = new Thickness(0);
            label.FontFamily = "montserratextrabold";
            label.FontAttributes = FontAttributes.None;

            var scaledDensity = DeviceDisplay.Current.MainDisplayInfo.Density; //= 2.75 on Pixel 5 emulator
            float fontSize = 20;
            label.FontSize = fontSize;
            Debug.WriteLine("SCALED DENSITY " + scaledDensity);
            string testString = "HELLO HELLO HELLO HELLO HELLO HELLO HELLO HELLO HELLO";
            int maxWidth = 1000;

            label.Text = testString;
            abs.Add(label);

            //==================================
            //MEASURE LABEL ON LAYOUT
            //==================================
#if ANDROID
            label.SizeChanged += delegate {

                //===============================================================
                //LABEL SIZE BY MAUI (WRONG!!!!) -> || 686.1818181818181 x 27 ||
                //===============================================================
                Debug.WriteLine("LABEL WIDTH " + label.Width + " H " + label.Height); //returns 686.1818181818181 x 27.272727272727273 (WRONG)

                //====================================================================================
                //LABEL SIZE BY STATIC LAYOUT (CORRECT, SAME AS ANDROID.NET) -> || 676.03125 x 28 ||
                //====================================================================================
                var fontManager = label.Handler.MauiContext.Services.GetRequiredService<IFontManager>();
                var font = Microsoft.Maui.Font.OfSize("montserratextrabold", fontSize); //has ENABLE SCALING = TRUE IN IT
                Android.Text.TextPaint textPaint = new();
                textPaint.TextSize = fontSize;
                textPaint.SetTypeface(font.ToTypeface(fontManager));  //https://stackoverflow.com/questions/39082855/how-do-i-use-a-custom-font-in-xamarin-android
                StaticLayout staticLayout = new(testString, textPaint, maxWidth, Android.Text.Layout.Alignment.AlignNormal, 1, 0, true); //should be "true" here to get same height as textview
                var staticLayoutSize = staticLayout.GetTextSizeAsSizeF(false); //IF THIS IS NOT FALSE RETURNS FULL WIDTH OF "MAX WIDTH = 5000"
                Debug.WriteLine("STATIC LAYOUT W " + staticLayoutSize.X + " H " + staticLayoutSize.Y + " top pad " + staticLayout.TopPadding + " bot pad " + staticLayout.BottomPadding); //returns 676.03125 x 28 same as Android


            };

#endif

        }
    }
    public static class GraphicsExtensions {
        //====================================================================================
        //FUNCTION COPIED FROM MAUI'S GRAPHICSEXTENSIONS FOR CHECKING LINES FOR MAX WIDTH
        //====================================================================================
        public static Vector2 GetTextSizeAsSizeF(this Android.Text.StaticLayout target, bool hasBoundedWidth) {
            // We need to know if the static layout was created with a bounded width, as this is what
            // StaticLayout.Width returns.
            if (hasBoundedWidth)
                return new Vector2(target.Width, target.Height);

            float maxWidth = 0;
            int lineCount = target.LineCount;

            for (int i = 0; i < lineCount; i++) {
                float lineWidth = target.GetLineWidth(i);
                if (lineWidth > maxWidth) {
                    maxWidth = lineWidth;
                }
            }
            return new Vector2(maxWidth, target.Height);
        }

    }
}
