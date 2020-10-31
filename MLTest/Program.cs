using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ML.Data;

namespace MLTest
{
    static class Program
    {
        public static GeneratorForm _generatorForm;
        public static InteractForm _interactForm;

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

            Application.Run(_generatorForm);
        }

    }
}
