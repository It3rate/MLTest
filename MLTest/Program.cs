using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ML.Data;
using MLTest.Forms;
using MLTest.Tylox;
using MLTest.Sim;

namespace MLTest
{
    static class Program
    {
        public static GeneratorForm _generatorForm;
        public static InteractForm _interactForm;
        public static TyloxForm _tyloxForm;
        public static SimForm _simForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _generatorForm = new GeneratorForm();
            _generatorForm.Hide();

            _interactForm = new InteractForm(_generatorForm.Generator);
            _interactForm.Hide();

            _tyloxForm = new TyloxForm(new TyloxRenderer());
            _tyloxForm.Hide();

            _simForm = new SimForm();
            _simForm.Hide();

            // Application.Run(_generatorForm);
            Application.Run(_simForm);
        }

    }
}
