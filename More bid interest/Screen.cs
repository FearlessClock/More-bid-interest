using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_bid_interest
{
    class Screen
    {
        List<Window> windows = new List<Window>();
        InteractiveWindow selector;
        string title;


        /// <summary>
        /// Always have to have a selectable screen
        /// The number of screens = n-1 nmbr of arrays because the last index is for the selector
        /// </summary>
        /// <param name="nmbrOfScreens"></param>
        /// <param name="positions"></param>
        /// <param name="widths"></param>
        /// <param name="heights"></param>
        /// <param name="texts"></param>
        public Screen(int nmbrOfScreens, Vector2[] positions, int[] widths, int[] heights, string[][] texts, List<Func<bool>> callbacks, string title)
        {
            this.title = title;
            int a = nmbrOfScreens + 1;
            if (a != positions.Length || a != widths.Length || a != heights.Length || a != texts.Length)
                Console.WriteLine("Error in the parameters");
            for (int i = 0; i < nmbrOfScreens; i++)
            {
                windows.Add(new Window(positions[i], widths[i], heights[i], texts[i]));
            }
            selector = new InteractiveWindow(positions[positions.Length - 1], widths[positions.Length - 1], heights[positions.Length - 1], texts[positions.Length - 1], callbacks);
        }

        public Vector2 GetPosOfWindow(int nmbr)
        {
            return windows[nmbr].pos;
        }

        public void Update(string[][] texts)
        {
            for (int i = 0; i < texts.Length-1; i++)
            {
                if(texts[i].Length > 0)
                    windows[i].texts = texts[i];
            }
            int selected = selector.Update();
            if (selected != -1)
            {
                selector.color = ConsoleColor.Yellow;
                //Call the callback function
            }
        }
        public void Draw()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].Draw(title);
            }
            selector.Draw();
        }

        /// <summary>
        /// Draw a square onto the screen
        /// </summary>
        /// <param name="pos">Left top position of the square</param>
        /// <param name="w">Width of the rectangle</param>
        /// <param name="h">Height of the rectangle</param>
        public void DrawSquare(Vector2 pos, int w, int h)
        {
            Texture2D texture = ContentPipe.LoadTexture("texture.PNG");
            GraphicsBuffer buffer = new GraphicsBuffer();
            buffer.indexBuffer = new uint[4];
            buffer.vertBuffer = new Vertex[4];
            buffer.IBO = GL.GenBuffer();
            buffer.VBO = GL.GenBuffer();
            for (int i = 0; i < buffer.indexBuffer.Length; i++)
            {
                buffer.indexBuffer[i] = (uint)i;
            }
            buffer.vertBuffer[0] = new Vertex(new Vector2(0, 0), new Vector2(0, 0));
            buffer.vertBuffer[1] = new Vertex(new Vector2(1, 0), new Vector2(1, 0));
            buffer.vertBuffer[2] = new Vertex(new Vector2(1, 1), new Vector2(1, 1));
            buffer.vertBuffer[3] = new Vertex(new Vector2(0, 1), new Vector2(0, 1));
            BufferFill(buffer);
            //Load the vert and index buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VBO);
            GL.VertexPointer(2, VertexPointerType.Float, Vertex.SizeInBytes, (IntPtr)0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes));
            GL.ColorPointer(4, ColorPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes * 2));
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.IBO);


            GL.BindTexture(TextureTarget.Texture2D, texture.ID);
            //Multiply the scale matrix with the modelview matrix
            Matrix4 mat = Matrix4.CreateTranslation(pos.X, pos.Y, 0);  //Create a translation matrix
            GL.MatrixMode(MatrixMode.Modelview);    //Load the modelview matrix, last in the chain of view matrices
            GL.LoadMatrix(ref mat);                 //Load the translation matrix into the modelView matrix
            mat = Matrix4.CreateScale(w, h, 0);
            GL.MultMatrix(ref mat);
            GL.DrawElements(PrimitiveType.Quads, buffer.indexBuffer.Length, DrawElementsType.UnsignedInt, 0);

        }

        private void BufferFill(GraphicsBuffer buf)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf.VBO);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.SizeInBytes * buf.vertBuffer.Length), buf.vertBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf.IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * (buf.indexBuffer.Length)), buf.indexBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        }
    }
}
