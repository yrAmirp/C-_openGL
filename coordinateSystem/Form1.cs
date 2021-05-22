using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets;

namespace coordinateSystem
{
    public partial class Form1 : Form
    {
        OpenGL gl;
        Random r;
        bool bParabola, bCos, refreshParams;
        float tX, tY, tZ, rX, rY, rZ;   //Переменные для перемещения и поворотов
        int mouseX, mouseY, countParabola, reverseCountParabola, countCos, reverseCountCos;
        float z, n, segment;  // параметры уравнения

        

       

        float[] coordParabola, coordCos; // Точки графиков








        public Form1()
        {
            InitializeComponent();
            label9.Text = trackBar1.Value + "x";
            this.MouseWheel += new MouseEventHandler(this_MouseWheel);
            r = new Random();
            tX = tY = rX = rY = rZ = 0;
            tZ = -10.0f;
            countParabola = 0;
            reverseCountParabola = 0;
            z = n = 1.0f;
            label2.Text = "X = " + tX;
            label3.Text = "Y = " + tY;
            label4.Text = "Z = " + tZ;

            coordParabola = new float[82];
            coordCos = new float[200];

            float x = -2.0f, y = 0.0f;
            for (int i = 0; i < coordParabola.Length; i += 2)
            {

                coordParabola[i] = x;
                y = (x * x);
                coordParabola[i + 1] = y;
                x += 0.1f;
            }




        }


        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
            gl = openGLControl1.OpenGL;
            gl.ClearColor(0.72f, 0.72f, 0.75f, 0.5f);
            gl.Clear(OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_COLOR_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Translate(tX, tY, tZ);
            gl.Rotate(rX, rY, rZ);
            label2.Text = "X = " + tX;
            label3.Text = "Y = " + tY;
            label4.Text = "Z = " + tZ;

           

            gl.Enable(OpenGL.GL_BLEND);   //Включить прозрачность
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA); //Функция расчета прозрачности

            gl.LineWidth(2.0f);

            gl.Begin(OpenGL.GL_LINES);
            //axisX
            gl.Color(0.55f, 0.0f, 0.0f, 0.5f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(2.0f, 0.0f, 0.0f);
            //axisY
            gl.Color(0.0f, 0.55f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 2.0f, 0.0f);
            //axisZ
            gl.Color(0.0f, 0.0f, 0.55f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 2.0f);

            gl.End();

            gl.LineWidth(0.5f);
            gl.Color(1.0f, 1.0f, 1.0f, 0.2f);
            gl.LineStipple(5, 0xAAAA);
            gl.Enable(OpenGL.GL_LINE_STIPPLE);
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(1.0f, 1.0f, 1.0f, 0.7f);

            //Линии XY
            for (float i = -2; i <= 2.1; i += 0.2f)
            {
                gl.Vertex(i, -2, 0);
                gl.Vertex(i, 2, 0);
            }
            for (float i = -2; i <= 2.1; i += 0.2f)
            {
                gl.Vertex(-2, i, 0);
                gl.Vertex(2, i, 0);
            }
            //Линии XZ

            for (float i = -2; i <= 2.1; i += 0.2f)
            {
                gl.Vertex(i, 0, -2);
                gl.Vertex(i, 0, 2);
            }
            for (float i = -2; i <= 2.1; i += 0.2f)
            {
                gl.Vertex(-2, 0, i);
                gl.Vertex(2, 0, i);
            }
            //Линии YZ
            for (float i = -2; i <= 2.1; i += 0.2f)
            {
                gl.Vertex(0, i, -2);
                gl.Vertex(0, i, 2);
            }
            for (float i = -2; i <= 2.1; i += 0.2f)
            {
                gl.Vertex(-0, -2, i);
                gl.Vertex(0, 2, i);
            }
            gl.End();
            gl.Disable(OpenGL.GL_LINE_STIPPLE);



            //Прозрачная плоскость//
            gl.Begin(OpenGL.GL_QUADS);
            //Плоскость XY
            gl.Color(0.0f, 0.0f, 1.0f, 0.1f);
            gl.Vertex(-2.0f, -2.0f, 0.0f);
            gl.Vertex(-2.0f, 2.0f, 0.0f);
            gl.Vertex(2.0f, 2.0f, 0.0f);
            gl.Vertex(2.0f, -2.0f, 0.0f);
            //Плоскость XZ
            gl.Color(1.0f, 0.0f, 0.0f, 0.1f);
            gl.Vertex(-2.0f, 0.0f, -2.0f);
            gl.Vertex(-2.0f, 0.0f, 2.0f);
            gl.Vertex(2.0f, 0.0f, 2.0f);
            gl.Vertex(2.0f, 0.0f, -2.0f);
            //Плоскость YZ
            gl.Color(0.0f, 1.0f, 0.0f, 0.1f);
            gl.Vertex(0.0f, -2.0f, -2.0f);
            gl.Vertex(0.0f, -2.0f, 2.0f);
            gl.Vertex(0.0f, 2.0f, 2.0f);
            gl.Vertex(0.0f, 2.0f, -2.0f);

            gl.End();

            //График параболы//
            gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
            gl.VertexPointer(2, 0, coordParabola);
            gl.Translate(0, 0, 0);

            gl.Color(0.3f, 0.3f, 1.0f, 1.0f);
            gl.LineWidth(3.0f);
            if (bParabola)
            {
                gl.Begin(OpenGL.GL_LINE_STRIP);

                for (int i = reverseCountParabola; i < countParabola; i++)
                {

                    gl.ArrayElement(i);

                }

                gl.End();
            }


            //График синусоиды//

            // Подготовка графика, задание координат

            float y1 = 0.0f, x1 = -5.0f;
            if (refreshParams)
            {
                coordCos = new float[200 * (int)n];
                segment = 0.1f / n;
                refreshParams = false;
                
            }
            for (int i = 0; i < coordCos.Length; i += 2)
            {
                coordCos[i] = x1;
                y1 = z * (float)Math.Cos(x1 * n);
                coordCos[i + 1] = y1;
                x1 += segment;

            }

            gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
            gl.VertexPointer(2, 0, coordCos);
            gl.Translate(0, 0, 2);

            gl.Color(1.0f, 0.3f, 0.2f, 1.0f);
            gl.LineWidth(3.0f);

            // Если стоит галочка то нарисовать график
            if (bCos)
            {
                gl.Begin(OpenGL.GL_LINE_STRIP);


                for (int i = reverseCountCos; i < countCos; i++)
                {

                    gl.ArrayElement(i);

                }

                gl.End();
            }

        }

        private void openGLControl1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    {
                        tX += 0.1f;
                    }
                    break;

                case Keys.D:
                    {
                        tX -= 0.1f;
                    }
                    break;
                case Keys.W:
                    {
                        tY -= 0.1f;
                    }
                    break;
                case Keys.S:
                    {
                        tY += 0.1f;
                    }
                    break;
                case Keys.Z:
                    {
                        tZ -= 0.1f;
                    }
                    break;
                case Keys.X:
                    {
                        tZ += 0.1f;
                    }
                    break;


            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                bCos = true;
                refreshParams = true;
            }
            else
            {
                bCos = false;
                refreshParams = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) bParabola = true;
            else bParabola = false;
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            label5.Text = "{" + (e.X - openGLControl1.Width / 2).ToString() + " ; " + (-(e.Y - openGLControl1.Height / 2)).ToString() + "}";

            if (e.Button == MouseButtons.Left)
            {

                if (mouseX < e.X - openGLControl1.Width / 2)
                {
                    rY += 1.3f;
                    mouseX = e.X - openGLControl1.Width / 2;
                }
                if (mouseX > e.X - openGLControl1.Width / 2)
                {
                    rY -= 1.3f;
                    mouseX = e.X - openGLControl1.Width / 2;
                }

                if (mouseY < e.Y - openGLControl1.Height / 2)
                {
                    rX += 1.3f;
                    mouseY = e.Y - openGLControl1.Height / 2;
                }
                if (mouseY > e.Y - openGLControl1.Height / 2)
                {
                    rX -= 1.3f;
                    mouseY = e.Y - openGLControl1.Height / 2;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            rX = rY = rZ = 0;
            tX = tY = 0;
            tZ = -10;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bParabola)
            {
                if (countParabola < coordParabola.Length / 2)
                    countParabola += 1;
                else
                    reverseCountParabola += 1;
                if (reverseCountParabola >= coordParabola.Length / 2)
                {
                    countParabola = 0;
                    reverseCountParabola = 0;

                }
            }
            if (bCos)
            {
                if (countCos < coordCos.Length / 2)
                    countCos += 1;
                else
                    reverseCountCos += 1;
                if (reverseCountCos >= coordCos.Length / 2)
                {
                    countCos = 0;
                    reverseCountCos = 0;
                    refreshParams = true;

                }
            }


        }

        void this_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                tZ += 0.2f;
            if (e.Delta < 0)
                tZ -= 0.2f;

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            n = (float)numericUpDown2.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            z = (float)numericUpDown1.Value;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = 50 / trackBar1.Value;
            label9.Text = trackBar1.Value + "x";
        }
    }
}
