using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using SharpGL.SceneGraph;
using Microsoft.Win32;
using System.Text.RegularExpressions;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi
        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi
        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Skeleton"), "Skeleton.3DS", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;
                case Key.E: if (m_world.Animation) break; m_world.RotationX += 5.0f; break;
                case Key.D: if (m_world.Animation) break; m_world.RotationX -= 5.0f; break;
                case Key.F: if (m_world.Animation) break; m_world.RotationY -= 5.0f; break;
                case Key.S: if (m_world.Animation) break; m_world.RotationY += 5.0f; break;
                case Key.Add: if (m_world.Animation) break; m_world.SceneDistance += 50.0f; break;
                case Key.Subtract: if (m_world.Animation) break; m_world.SceneDistance -= 50.0f; break;
                case Key.V: if (m_world.Animation) break; m_world.EarthRotation = 0.0f; m_world.Animation = true; break;
                case Key.M: if (m_world.Animation) break; m_world.ChangeLight(openGLControl.OpenGL); break;
                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool)opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK);
                        }
                    }
                    break;
                }
                lbl.Content = m_world.SelectedLightSourceType.ToString();
        }

        private void visina_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (m_world.Animation)
                this.visina.IsEnabled = false;
            else
            {
                bool ok = new Regex("[0-9]").IsMatch(e.Text);

                if (ok)
                {
                    m_world.Visina = int.Parse(this.visina.Text + e.Text);
                }

                e.Handled = !ok;
            }
        }

        private void brzina_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (m_world.Animation)
                this.brzina.IsEnabled = false;
            else
            {
                bool ok = new Regex("[0-9]").IsMatch(e.Text);

                if (ok)
                {
                    m_world.Brzina = int.Parse(this.brzina.Text + e.Text);
                }

                e.Handled = !ok;
            }
        }   
    }
}
