using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace STGCodeChallenge4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Evaluate a mathematical expression. For example, if str were "2+(3-1)*3" the output should be 8. Another example: if str were "(2-0)(6/2)" the output should be 6. 
        /// There can be parenthesis within the string so you must evaluate it properly according to the rules of arithmetic. The string will contain the operators: +, -, /, *, (, and ). 
        /// If you have a string like this: #/#*# or #+#(#)/#, then evaluate from left to right. So divide then multiply, and for the second one multiply, divide, then add. 
        /// The evaluations will be such that there will not be any decimal operations, so you do not need to account for rounding and whatnot.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            string expression = "2+(3-1)*3";
            txtExpression.Text = expression;
        }

        #region Option 1: Code to generate c# class on the fly and calculate the value of a given expression
        /// <summary>
        /// Builds a .cs class on the fly and adds a new method for each selected linq query.
        /// Compile and Run each method after adding it, displaying the results in the txtResultsBox on the UI.
        /// </summary>
        private Decimal createDynamicFunctions(string expression)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                var c = new CodeDom();
                c.AddReference(@"System.Core.dll").AddNamespace("STGCodeChallenge4");
                c.AddUsing("System");
                c.AddUsing("System.Collections.Generic");
                c.AddUsing("System.Linq");
                c.AddReference("System.Xml.dll").AddReference("System.Xml.Linq.dll").AddUsing("System.Xml.Linq");
                c.AddUsing("System.Text");
                c.AddUsing("System.Text.RegularExpressions");//.AddReference("System.Text.dll").AddReference("System.Text.RegularExpressions.dll").
                c.AddClass(c.Class("ExpressionEvaluator"));

                //Add a method to the class to calculate the value of the expression, compile the class and run the newly added method.
                c.AddMethod(c.Method("Decimal", "calculateValue", "", "return " + expression + ";"));
                string code = c.GenerateCode();
                try
                {
                    var method = c.Compile().GetType("STGCodeChallenge4.ExpressionEvaluator").GetMethod("calculateValue");
                    Decimal results = (Decimal)method.Invoke(null, null);
                    return results;
                }
                catch (Exception ex)
                {
                    //txtResultsBox.Text += "!!!!!!!!!!!!!!!!!!!!!****Invalid expression****!!!!!!!!!!!!!!!!!!!!!\n";
                }
            }
            else
            {
                //txtResultsBox.Text += "No expression was entered.\n";
            }
            return new Decimal(0.0);
        }
        #endregion

        /// <summary>
        /// Process Expression button was pressed.  Begins the processes to evaluate the expression using 2 different methods.
        /// </summary>
        /// <param name="sender">The button</param>
        /// <param name="e">The click event</param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtExpression.Text))
            {
                #region option 1: dynamic class
                lblDynamicFinalValue.Content = createDynamicFunctions(normalizeExpression(txtExpression.Text));
                #endregion

                #region option 2:shunting yard algorithm
                lblShuntingYardFinalValue.Content = new ShuntingYard(normalizeExpression(txtExpression.Text)).processExpression();
                #endregion
            }
        }

        /// <summary>
        /// Normalizes the expression by adding * in where () are used for multiplication.
        /// </summary>
        /// <param name="originalExpression">The experssion as entered by the user</param>
        /// <returns>A normalized version of the expression ready for processing</returns>
        private string normalizeExpression(string originalExpression)
        {
            string normalizedExpression = originalExpression.Replace(")(", ") * (");
            normalizedExpression = Regex.Replace(normalizedExpression, @"(\d+)([(]\d+)", "$1 * $2"); 
            return normalizedExpression;
        }
    }
}
