using System;
using System.IO;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Print;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using DemoPDFXamarin.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace DemoPDFXamarin
{
    [Activity(Label = "Droid Print Demo", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IPermissionListener
    {
        Button btn_create_pdf;
        public static string fileName = "Order.pdf";

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            btn_create_pdf = FindViewById<Button>(Resource.Id.btn_create_pdf);
            await Common.Common.WriteFileStorageAsync(this, "FuturaStdMedium.otf");
            Dexter.WithActivity(this)
                .WithPermission(Manifest.Permission.WriteExternalStorage)
                .WithListener(this)
                .Check();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            Toast.MakeText(this, "You must accept this permission", ToastLength.Short).Show();
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            btn_create_pdf.Click += delegate
            {
                CreatePDFFILE(Common.Common.GetAppPath(this) + fileName);
            };
        }

        private void CreatePDFFILE(string v)
        {
            //Check available
            if (new Java.IO.File(v).Exists())
            {
                new Java.IO.File(v).Delete();
            }
            try
            {
                Document document = new Document();
                //Save
                PdfWriter.GetInstance(document, new FileStream(v, FileMode.Create));
                //Open
                document.Open();
                //Settings
                document.SetPageSize(PageSize.A4);
                document.AddAuthor("Ando");
                document.AddCreator("Ando");

                //Font Setting
                Color colorAccent = new Color(0, 153, 204, 255);
                float fontSize = 20.0f, valueFontSize = 26.0f;
                BaseFont fontName = BaseFont.CreateFont(Common.Common.GetFilePath(this, "FuturaStdMedium.otf"),
                    "UTF-8",
                    BaseFont.EMBEDDED);

                //Create Title of Document
                Font titleFont = new Font(fontName, 36.0f, Font.NORMAL, Color.BLACK);
                AddNewItem(document, "ORDER DETAILS", Element.ALIGN_CENTER, titleFont);

                //AddMore
                Font orderNumberFont = new Font(fontName, fontSize, Font.NORMAL, colorAccent);
                AddNewItem(document, "Order No: ", Element.ALIGN_LEFT, orderNumberFont);

                Font orderNumberValueFont = new Font(fontName, valueFontSize, Font.NORMAL, colorAccent);
                AddNewItem(document, "23412", Element.ALIGN_LEFT, orderNumberValueFont);

                AddLineSeparator(document);

                AddNewItem(document, "Order Date", Element.ALIGN_LEFT, orderNumberFont);
                AddNewItem(document, "12/12/2020", Element.ALIGN_LEFT, orderNumberValueFont);

                AddLineSeparator(document);

                //Add Product order Details
                AddLineSpace(document);
                AddNewItem(document, "Product Detail", Element.ALIGN_CENTER, titleFont);
                AddLineSeparator(document);

                //Item 1
                AddNewItemWithLeftAndRight(document, "Apple 30", "(0.0%)", titleFont, orderNumberFont);
                AddNewItemWithLeftAndRight(document, "5.0*100", "500.0", titleFont, orderNumberFont);
                AddLineSeparator(document);

                //Item 2
                AddNewItemWithLeftAndRight(document, "Apple 20", "(0.0%)", titleFont, orderNumberFont);
                AddNewItemWithLeftAndRight(document, "5.0*100", "500.0", titleFont, orderNumberFont);
                AddLineSeparator(document);

                //Total
                AddLineSpace(document);
                AddLineSpace(document);

                AddNewItemWithLeftAndRight(document, "Total", "1 000.00", titleFont, orderNumberFont);

                document.Close();
                Toast.MakeText(this, "Success create PDF", ToastLength.Short).Show();

                PrintPDF();


            }
            catch (FileNotFoundException e)
            {
                Log.Debug("[ERROR]", " " + e.Message);
            }
            catch (DocumentException e)
            {
                Log.Debug("[ERROR]", " " + e.Message); ;
            }
            catch (IOException e)
            {
                Log.Debug("[ERROR]", " " + e.Message);
            }
        }

        private void PrintPDF()
        {
            PrintManager printManager = (PrintManager)GetSystemService(Context.PrintService);
            try
            {
                PrintDocumentAdapter adapter = new PrintPDFAdapter(this, Common.Common.GetAppPath(this)+fileName);
                printManager.Print("Document", adapter, new PrintAttributes.Builder().Build());
            }
            catch (Exception e)
            {
                Log.Debug("[ERROR]", " " + e.Message);
            }
        }

        private void AddNewItemWithLeftAndRight(Document document, string leftText, string rightText, Font leftFont, Font rightFont)
        {
            Chunk chunkleft = new Chunk(leftText, leftFont);
            Chunk chunkright = new Chunk(rightText, rightFont);
            Paragraph p = new Paragraph(chunkleft);
            p.Add(new Chunk(new VerticalPositionMark()));
            p.Add(chunkright);
            document.Add(p);
        }

        private void AddLineSeparator(Document document)
        {
            LineSeparator lineSeparator = new LineSeparator();
            lineSeparator.LineColor = new Color(0, 0, 0, 68);
            AddLineSpace(document);
            document.Add(new Chunk(lineSeparator));
            AddLineSpace(document);
        }

        private void AddLineSpace(Document document)
        {
            document.Add(new Paragraph(("")));
        }

        private void AddNewItem(Document document, string text, int align, Font font)
        {

            Chunk chunk = new Chunk(text, font);
            Paragraph p = new Paragraph(chunk);
            p.Alignment = align;
            document.Add(p);
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {
            throw new NotImplementedException();
        }
    }
}
