// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using SharpGL.SceneGraph.Primitives;
using System.Drawing;
using System.Drawing.Imaging;
using SharpGL;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi
        //atributi vezani za teksture...
        private enum TextureObjects { Floor = 0, Stairs, Postolje, Platforma };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;

        private string[] m_textureFiles = { "..//..//slike//plocice2.jpg", "..//..//slike//metal.jpg", "..//..//slike//zlato.jpg", "..//..//slike//metal2.jpg" };

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 5000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width = 0;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height = 0;

        private int visina = 0;

        private bool animation = false;

        private float m_earthRotation = -1.0f;

        private float m_earthTranslation = 0.0f;

        private float brzina = 1;

        public enum LightSourceType
        {
            Zuto,
            Crveno,
            Zeleno,
            Plavo
        };

        private LightSourceType m_selectedLightSourceType = LightSourceType.Zuto;

        #endregion Atributi
        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public int Visina
        {
            get { return visina; }
            set { visina = value; }
        }

        public bool Animation
        {
            get { return animation; }
            set { animation = value; }
        }

        public float EarthRotation
        {
            get { return m_earthRotation; }
            set { m_earthRotation = value; }
        }

        public float EarthTranslation
        {
            get { return m_earthTranslation; }
            set { m_earthTranslation = value; }
        }

        public float Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }

        #endregion Properties

        public LightSourceType SelectedLightSourceType
        {
            get { return m_selectedLightSourceType; }
            set { m_selectedLightSourceType = value; }
        }
        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_SMOOTH);

            //Depth test ukljucen
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            //Sakrivanje nevidljivih povrsina
            gl.Enable(OpenGL.GL_CULL_FACE);
            //Podesavanje orijentacije poligona
            gl.FrontFace(OpenGL.GL_CW);

            //Osvetljenje
            float[] light0pos = new float[] { 150f, 150f, -45f, 1f };
            float[] light0ambient = new float[] { 0.576471f, 0.858824f, 0.439216f, 1.0f };
            float[] light0diffuse = new float[] { 0.576471f, 0.858824f, 0.439216f, 1.0f };
            float[] light0specular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180f); //tackasti svetlosni izvor
            gl.Enable(OpenGL.GL_LIGHT0);

            float[] light1pos = new float[] { 40.0f, 1550.0f, -5050.0f, 1f };
            float[] light1ambient = new float[] { 1f, 0f, 0f, 1.0f };
            float[] light1diffuse = new float[] { 1.0f, 0.0f, 0f, 1.0f };
            float[] light1specular = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
            float[] light1direction = new float[] { 40.0f, 43.0f, -5050.0f, 0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, light1direction); //reflektorski izvor svetlosti
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 40f);
            gl.Enable(OpenGL.GL_LIGHT1);

            //Ulazak u color tracking rezim
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE); //ambijentalna i difuzna komponenta materijala
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_NORMALIZE);

            //ulazak u rezim rada sa teksturama
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);	//repeat po obe ose	
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST); //najblizi sused filtriranje
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST); //najblizi sused filtriranje
                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD); //stapanje teksture sa materijalom

                image.UnlockBits(imageData);
                image.Dispose();
            }
           
            m_scene.LoadScene();
            m_scene.Initialize();
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;

            //kreira viewport po celom prozoru
            gl.Viewport(0, 0, m_width, height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(50.0f, (double)width / (double)height, 0.5f, 10000f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            String[] message = new string[]
            {
                "Predmet:  Racunarska grafika",
                "Sk.god:      2019/20",
                "Ime:         Igor",
                "Prezime:     Srajer",
                "Sifra zad:   11.1"
            };
            //--------------------------------------------------------------------------------------------
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Viewport(0, 0, m_width, m_height);
            gl.LoadIdentity();

            //Podesavanje kamere 25.0f, 43.0f, -50.0f -1000, -1000, 0
            gl.LookAt(0, 2500, -9000, 0, -300, 55, 0, 1, 0);

            gl.Translate(0.0f, -300.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 1.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Scale(10.0f, 10.0f, 15.0f);
            gl.Enable(OpenGL.GL_POINT_SMOOTH);

            //restrikcija rotacije
            if(m_xRotation >= 40.0f)
            {
                m_xRotation = 40.0f;
            }

            if(m_xRotation <= -160.0f)
            {
                m_xRotation = -160.0f;
            }

            gl.PushMatrix();

            //animacija
            if (m_earthRotation >= 0)
            {
                if (m_earthRotation < 50)
                {
                    gl.Translate(0.0f, 0.0f, m_earthRotation);
                    m_earthRotation += 5;
                }
                else if (m_earthRotation >= 50 && m_earthRotation < 105)
                {
                    gl.Translate(0.0f, m_earthTranslation + visina, m_earthRotation);
                    m_earthTranslation += 19;
                    m_earthRotation += 5;
                }
                else if (m_earthRotation >=105 && m_earthRotation < 170)
                {
                    gl.Translate(0.0f, m_earthTranslation, m_earthRotation);
                    m_earthRotation += 5;
                }
                else if(m_earthRotation >= 170)
                {
                    System.Threading.Thread.Sleep(2000);
                    m_earthRotation = -1;
                    m_earthTranslation = -1;
                    animation = false;
                }
            }

            gl.Color(0.196078, 0.196078, 0.8);
            gl.Translate(25.0f, 43.0f, -50.0f);
            gl.Rotate(90.0f, 1.0f, 0.0f, 0.0f);
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Platforma]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.Scale(1.0f, 1.0f, visina+1);
            gl.Translate(0.0f, 0.0f, -visina*14);
            m_scene.Draw();
            gl.PopMatrix();
            gl.PopMatrix();
            
            //stub
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Platforma]);
            gl.Color(0.55, 0.09, 0.09);
            DrawCylinder(gl);
            gl.PopMatrix();

            //postolje
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Postolje]);
            gl.Color(0.5, 1.5, 0.5);
            DrawPlatform(gl);
            DrawGround(gl);
            gl.PopMatrix();

            //stepenice 1
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Stairs]);
            gl.Color(0.137255, 0.419608, 0.556863);
            gl.Translate(10.0f, 0.0f, 0.0f);
            DrawStairs(gl);
            gl.PopMatrix();

            //stepenice 2
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Stairs]);
            gl.Translate(-45.0f, 0.0f, 0.0f);
            DrawStairs(gl);
            gl.PopMatrix();

            //stair fence 1
            gl.PushMatrix(); 
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Postolje]);
            gl.Color(0.59, 0.41, 0.31);
            gl.Translate(36.0f, 10.0f, -10.0f);
            gl.Rotate(14.0f, 1.0f, 0.0f, 0.0f);
            DrawStairFence(gl);
            gl.PopMatrix();

            //stair fence 2
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Postolje]);
            gl.Color(0.59, 0.41, 0.31);
            gl.Translate(-18.0f, 10.0f, -10.0f);
            gl.Rotate(14.0f, 1.0f, 0.0f, 0.0f);
            DrawStairFence(gl);
            gl.PopMatrix();

            //stair fence 3
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Postolje]);
            gl.Color(0.59, 0.41, 0.31);
            gl.Translate(-70.0f, 10.0f, -10.0f);
            gl.Rotate(14.0f, 1.0f, 0.0f, 0.0f);
            DrawStairFence(gl);
            gl.PopMatrix();

            
            //tekst
            gl.PushMatrix();
            SharpGL.FontBitmaps font = new FontBitmaps();
            for (int i = 0; i < message.Length; i++)
            {
                gl.Viewport(26 * m_width / 30, ((-i + 5) * m_height) / 30, m_width / 5, m_height / 5);
                font.DrawText(gl, 0, 0, 1.0f, 0.0f, 0.0f, "Verdana italic", 10.0f, message[i]);
            }
            
            
            gl.PopMatrix();
            gl.Viewport(0, 0, m_width, m_height);

            //oznaci kraj iscrtavanja
            gl.Flush();
        }

        //metoda za crtanje podloge
        public void DrawGround(OpenGL gl)
        {
            gl.PushMatrix();

            //plava boja
            gl.Color(0.196078, 0.196078, 0.8);

            //dodela teksture plocica i stapanje sa materijalom
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Floor]);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            gl.LoadIdentity();
            gl.PushMatrix();
            gl.Scale(10f, 10f, 10f); // 10x veci broj plocica 


            gl.Begin(OpenGL.GL_QUADS);

            gl.Normal(LightingUtilities.FindFaceNormal(-1000.0f, 0.0f, -1000.0f, 1000.0f, 0.0f, -1000.0f,
                                            1000.0f, 0.0f, 1000.0f));
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-1000.0f, 0.0f, -1000.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(1000.0f, 0.0f, -1000.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(1000.0f, 0.0f, 1000.0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-1000.0f, 0.0f, 1000.0f);                    
            gl.End();

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PopMatrix();

            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.PopMatrix();
        }

        //noseci stub
        public void DrawCylinder(OpenGL gl)
        {
            gl.PushMatrix(); //crtanje kvadra
            gl.Color(0.90, 0.91, 0.98);
            gl.Translate(0.0f, 50.0f, 120.0f);
            gl.Scale(48.0f, 140.0f, -48.0f);

            Cube cube = new Cube();
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }

        //platforma sa ogradama
        public void DrawPlatform(OpenGL gl)
        {
            gl.PushMatrix();// postolje
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Color(0.96, 0.80, 0.69);
            gl.Translate(0.0f, 195.0f, 120.0f);
            gl.Scale(60.0f, 10.0f, -60.0f);
            Cube cube2 = new Cube();
            cube2.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();// ograda 1
            gl.Color(0.59, 0.41, 0.31);
            gl.Translate(-55.0f, 215.0f, 110.0f);
            gl.Scale(5.0f, 15.0f, -65.0f);
            Cube cube3 = new Cube();
            cube3.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();// ograda 2
            gl.Color(0.59, 0.41, 0.31);
            gl.Translate(55.0f, 215.0f, 110.0f);
            gl.Scale(5.0f, 15.0f, -65.0f);
            Cube cube4 = new Cube();
            cube4.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();// ograda 3
            gl.Color(0.59, 0.41, 0.31);
            gl.Translate(0.0f, 215.0f, 172.0f);
            gl.Scale(60.0f, 15.0f, -7.0f);
            Cube cube5 = new Cube();
            cube5.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }

        //ograda od stepenica
        public void DrawStairFence(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(17.0f, 65.0f, 10.0f);
            gl.Scale(8.0f, 160.0f, -15.0f);
        
            Cube cube = new Cube();
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }

        //stepenice
        public void DrawStairs(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(17.0f, 6.0f, 10.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube = new Cube();
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
            
            gl.PushMatrix();
            gl.Translate(17.0f, 22.0f, 14.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube1 = new Cube();
            cube1.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 38.0f, 18.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube2 = new Cube();
            cube2.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 54.0f, 22.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube3 = new Cube();
            cube3.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 70.0f, 26.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube4 = new Cube();
            cube4.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 86.0f, 30.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube5 = new Cube();
            cube5.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 102.0f, 34.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube6 = new Cube();
            cube6.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 118.0f, 38.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube7 = new Cube();
            cube7.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 134.0f, 42.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube8 = new Cube();
            cube8.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 150.0f, 46.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube9 = new Cube();
            cube9.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 166.0f, 50.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube10 = new Cube();
            cube10.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 182.0f, 54.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube11 = new Cube();
            cube11.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(17.0f, 198.0f, 58.0f);
            gl.Scale(20.0f, 8.0f, -7.0f);

            Cube cube12 = new Cube();
            cube12.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }

        //izbor komponente tackastog osvetljenja
        public void ChangeLight(OpenGL gl)
        {
            SelectedLightSourceType = (LightSourceType)(((int)m_selectedLightSourceType + 1) % Enum.GetNames(typeof(LightSourceType)).Length);
            switch (m_selectedLightSourceType)
            {
                case LightSourceType.Zuto:

                    //Osvetljenje
                    float[] light3pos = new float[] { 150f, 150f, -45f, 1f };
                    float[] light3ambient = new float[] { 0.576471f, 0.858824f, 0.439216f, 1.0f };
                    float[] light3diffuse = new float[] { 0.576471f, 0.858824f, 0.439216f, 1.0f };
                    float[] light3specular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light3pos);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light3ambient);
                    //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light3diffuse);
                    //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light3specular);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180f); //tackasti svetlosni izvor
                    gl.Enable(OpenGL.GL_LIGHT0);
                    break;

                case LightSourceType.Crveno:

                    float[] light0pos = new float[] { 150f, 150f, -45f, 1f };
                    float[] light0ambient = new float[] { 1.0f, 0f, 0f, 1.0f };
                    float[] light0diffuse = new float[] { 1.0f, 0f, 0f, 1.0f };
                    float[] light0specular = new float[] { 1.0f, 0f, 0f, 1.0f };

                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
                    //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
                   // gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
                    gl.Enable(OpenGL.GL_LIGHT0);
                    break;

                case LightSourceType.Zeleno:

                    float[] light1pos = new float[] { 150f, 150f, -45f, 1f };
                    float[] light1ambient = new float[] { 0f, 1f, 0f, 1.0f };
                    float[] light1diffuse = new float[] { 0f, 1f, 0f, 1.0f };
                    float[] light1specular = new float[] { 0f, 1f, 0f, 1.0f };

                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light1pos);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light1ambient);
                    //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light1diffuse);
                    //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light1specular);
                    gl.Enable(OpenGL.GL_LIGHT0);
                    break;

                case LightSourceType.Plavo:


                    float[] light2pos = new float[] { 150f, 150f, -45f, 1f };
                    float[] light2ambient = new float[] { 0f, 0f, 1f, 1.0f };
                    float[] light2diffuse = new float[] { 0f, 0f, 1f, 1.0f };
                    float[] light2specular = new float[] { 0f, 0f, 1f, 1.0f };

                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light2pos);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light2ambient);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light2diffuse);
                    gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light2specular);
                    gl.Enable(OpenGL.GL_LIGHT0);
                    break;
            }

        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}