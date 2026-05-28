namespace AutoRentimine.WindowsForms
{
    partial class RentalForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelSelection = new Panel();
            panelActive = new Panel();
            panelResult = new Panel();
            SuspendLayout();
            // 
            // panelSelection
            // 
            panelSelection.Location = new Point(1, 1);
            panelSelection.Name = "panelSelection";
            panelSelection.Size = new Size(798, 448);
            panelSelection.TabIndex = 0;
            // 
            // panelActive
            // 
            panelActive.Location = new Point(1, 1);
            panelActive.Name = "panelActive";
            panelActive.Size = new Size(798, 448);
            panelActive.TabIndex = 1;
            // 
            // panelResult
            // 
            panelResult.Location = new Point(1, 1);
            panelResult.Name = "panelResult";
            panelResult.Size = new Size(798, 448);
            panelResult.TabIndex = 2;
            // 
            // RentalForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panelActive);
            Controls.Add(panelResult);
            Controls.Add(panelSelection);
            Name = "RentalForm";
            Text = "RentalForm";
            ResumeLayout(false);
        }

        #endregion

        private Panel panelSelection;
        private Panel panelResult;
        private Panel panelActive;
    }
}