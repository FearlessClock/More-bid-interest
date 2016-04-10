using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.IO;
using System.Xml;
using TextToScreen;

namespace More_bid_interest
{
    struct Vertex
    {
        public Vector2 position;
        public Vector2 texCoord;
        public Vector4 color;

        public Color Color
        {
            get
            {
                return Color.FromArgb((int)(255 * color.W), (int)(255 * color.X), (int)(255 * color.Y), (int)(255 * color.Z));
            }
            set
            {
                this.color = new Vector4(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
            }

        }
        static public int SizeInBytes
        {
            get { return Vector2.SizeInBytes * 2 + Vector4.SizeInBytes; }
        }

        public Vertex(Vector2 position, Vector2 texCoord)
        {
            this.position = position;
            this.texCoord = texCoord;
            this.color = new Vector4(1, 1, 1, 1);
        }


    }
    class Game
    {
        public GameWindow window;
        Texture2D texture;
        Random rand = new Random();

        //Start of the vertex buffer
        GraphicsBuffer buffer = new GraphicsBuffer();
        
        public Game(GameWindow windowInput)
        {
            window = windowInput;

            window.Load += Window_Load;
            window.RenderFrame += Window_RenderFrame;
            window.UpdateFrame += Window_UpdateFrame;
            window.Closing += Window_Closing;
            Camera.SetupCamera(window, 30);
        }

        Screen MainScreen;
        Screen BidScreen;
        Screen EmpireScreen;

        int currentScreen = 0;

        GeneralBidControl bidControl = new GeneralBidControl();

        String[][] texts;

        TextToScreen.TextWriter tw = new TextToScreen.TextWriter("Alphabet\\");

        private void Window_Load(object sender, EventArgs e)
        {
            buffer.VBO = GL.GenBuffer();
            buffer.IBO = GL.GenBuffer();
            buffer.Empty();

            List<Func<bool>> callbacks = new List<Func<bool>>();
            callbacks.Add(GoToBid);
            callbacks.Add(GoToEmpire);
            callbacks.Add(Quit);

            CreateScreen("Content/MainScreen.xml", out MainScreen, callbacks);

            callbacks = new List<Func<bool>>();
            callbacks.Add(GoToMain);
            callbacks.Add(GoToEmpire);

            CreateScreen("Content/BidScreen.xml", out BidScreen, callbacks);


            callbacks = new List<Func<bool>>();
            callbacks.Add(GoToMain);
            callbacks.Add(GoToBid);
            callbacks.Add(UpgradePalace);

            CreateScreen("Content/EmpireScreen.xml", out EmpireScreen, callbacks);
            GeneralGameControl.setupItems("Content/Items.xml", rand);
            GeneralGameControl.setupBuyers("Content/Buyers.xml");
        }

        public void CreateScreen(string fileName, out Screen win, List<Func<bool>> callbacks)
        {
            //Change into xml code
            XmlDocument mainScreenSettings = new XmlDocument();
            mainScreenSettings.Load(fileName);
            XmlNode node = mainScreenSettings.SelectSingleNode("//Screen/Startup");

            int nmbrOfScreens = Convert.ToInt32(node.Attributes["NmbrOfScreens"].Value);
            string title = node.Attributes["Title"].Value;
            Vector2[] positions = new Vector2[nmbrOfScreens + 1];
            int[] W = new int[nmbrOfScreens + 1];
            int[] H = new int[nmbrOfScreens + 1];
            texts = new string[nmbrOfScreens + 1][];
            XmlNodeList nodes = mainScreenSettings.SelectNodes("//Screen/Windows/Window");
            for (int i = 0; i < nmbrOfScreens + 1; i++)
            {
                positions[i] = new Vector2(Convert.ToInt32(nodes[i].Attributes["X"].Value), Convert.ToInt32(nodes[i].Attributes["Y"].Value));
                W[i] = Convert.ToInt32(nodes[i].Attributes["W"].Value);
                H[i] = Convert.ToInt32(nodes[i].Attributes["H"].Value);
                string[] textPerScreen = nodes[i].Attributes["text"].Value.Split(':');
                texts[i] = new string[textPerScreen.Length];
                for (int j = 0; j < textPerScreen.Length; j++)
                {
                    texts[i][j] = textPerScreen[j];
                }
            }
            win = new Screen(nmbrOfScreens, positions, W, H, texts, callbacks, title);
            GeneralGameControl.StartStopwatch();
            GeneralEmpireControl.Setup();
        }

        private void BufferFill(GraphicsBuffer buf)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf.VBO);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.SizeInBytes * buf.vertBuffer.Length), buf.vertBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf.IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * (buf.indexBuffer.Length)), buf.indexBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        GraphicsBuffer[] buf;
        int mash = 0;
        private void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            Camera.CameraUpdate();
            Camera.CheckPresses();
            GeneralEmpireControl.BackgroundUpdate();
            switch(currentScreen)
            {
                case 0:
                    GetMainText();
                    MainScreen.Update(texts);
                    break;
                case 1:
                    bidControl.BidState();
                    GetBidText();
                    BidScreen.Update(texts);
                    break;
                case 2:
                    GetEmpireText();
                    EmpireScreen.Update(texts);
                    break;
            }
        }

        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            //Clear screen color
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Enable color blending, which allows transparency
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            //Blending everything for transparency
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //Create the projection matrix for the scene
            Matrix4 proj = Matrix4.CreateOrthographicOffCenter(0, window.Width, window.Height, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref proj);

            //Bind the texture that will be used
            GL.BindTexture(TextureTarget.Texture2D, texture.ID);

            //Enable all the different arrays
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            //Load the vert and index buffers
            /* GL.BindBuffer(BufferTarget.ArrayBuffer, buf[0].VBO);
             GL.VertexPointer(2, VertexPointerType.Float, Vertex.SizeInBytes, (IntPtr)0);
             GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes));
             GL.ColorPointer(4, ColorPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes * 2));
             GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf[0].IBO);

             //Create a scale matrux
             Matrix4 mat = Matrix4.CreateTranslation(0, 0, 0);  //Create a translation matrix
             GL.MatrixMode(MatrixMode.Modelview);    //Load the modelview matrix, last in the chain of view matrices
             GL.LoadMatrix(ref mat);                 //Load the translation matrix into the modelView matrix
             mat = Matrix4.CreateScale(1, 1, 0);
             GL.MultMatrix(ref mat);                     //Multiply the scale matrix with the modelview matrix
             GL.PushMatrix();
             GL.DrawElements(PrimitiveType.Quads, buffer.indexBuffer.Length, DrawElementsType.UnsignedInt, 0);
             */
            //tw.WriteToScreen(new Vector2(10, 10), texts[0][0], 300, 10);
            if (currentScreen == 0) 
                MainScreen.Draw();
            else if (currentScreen == 1)
            {
                BidScreen.DrawSquare(BidScreen.GetPosOfWindow(4), 100, 10);
                BidScreen.Draw();
            }
            else if (currentScreen == 2)
                EmpireScreen.Draw();

            //Flush everything 
            GL.Flush();
            //Write the new buffer to the screen
            window.SwapBuffers();
        }


        //Functions for each button
        #region button callback functions
        public bool Quit()
        {
            window.Close();
            return true;
        }

        public bool GoToMain()
        {
            currentScreen = 0;
            return true;
        }
        public bool GoToBid()
        {
            currentScreen = 1;
            bidControl.Setup(Player.GetPlayerInventory);
            return true;
        }
        public bool GoToEmpire()
        {
            currentScreen = 2;
            return true;
        }

        public bool UpgradePalace()
        {
            if(GeneralEmpireControl.LevelUpPalace())
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Menu text functions
        private void GetMainText()
        {
            texts = new string[3][];
            texts[0] = new string[] { "The next round starts in:", GeneralGameControl.GetTimeTillNextBid.ToString() };
            texts[1] = new string[] { "Your empire is doing well", "Your palace is level: " + GeneralEmpireControl.GetPalaceLevel.ToString(), "You have " + Player.Money + " dollars" };
            texts[2] = new string[0];
        }
        private void GetBidText()
        {
            texts = new string[6][];
            texts[0] = bidControl.GetNextItemName;
            texts[1] = bidControl.GetBuyerNames;
            texts[2] = new string[] { "Remaining time: " + bidControl.GetRemainingTime, "Money: " + Player.Money };
            texts[3] = bidControl.GetDebugText;
            texts[4] = bidControl.GetMashText;// bidControl.GetMashText;
            texts[5] = new string[0];
        }
        private void GetEmpireText()
        {
            texts = new string[3][];
            texts[0] = new string[0];
            texts[1] = new string[] { "Your empire is currently at level :" + GeneralEmpireControl.GetPalaceLevel.ToString() };
            texts[2] = new string[0];
        }
        #endregion
    }
}
