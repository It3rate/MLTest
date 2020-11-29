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
        private static int _formIndex = 2;
        private static Form ActiveForm;
        private static List<Form> Forms;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var genForm = new GeneratorForm();
            Forms = new List<Form>
            {
	            genForm,
	            new InteractForm(genForm.Generator),
	            new TyloxForm(),
	            new SimForm(),
	            new VisForm(),
            };

            NextForm();

            Application.Run(ActiveForm);
        }

        public static void NextForm()
        {
	        _formIndex++;
	        if (_formIndex >= Forms.Count)
	        {
		        _formIndex = 0;
	        }

	        foreach (var form in Forms)
	        {
		        form.StartPosition = FormStartPosition.CenterScreen;
		        if (form.Visible)
		        {
			        form.Hide();
		        }
	        }

            ActiveForm = Forms[_formIndex];
            ActiveForm.Show();
        }
    }
}
