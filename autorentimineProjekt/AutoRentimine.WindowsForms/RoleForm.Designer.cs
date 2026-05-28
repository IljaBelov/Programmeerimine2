namespace AutoRentimine.WindowsForms
{
    partial class RoleForm
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
            btnModerator = new Button();
            btnClient = new Button();
            SuspendLayout();
            // 
            // btnModerator
            // 
            btnModerator.Location = new Point(419, 159);
            btnModerator.Name = "btnModerator";
            btnModerator.Size = new Size(94, 29);
            btnModerator.TabIndex = 0;
            btnModerator.Text = "btnModerator";
            btnModerator.UseVisualStyleBackColor = true;
            // 
            // btnClient
            // 
            btnClient.Location = new Point(549, 159);
            btnClient.Name = "btnClient";
            btnClient.Size = new Size(94, 29);
            btnClient.TabIndex = 1;
            btnClient.Text = "btnClient";
            btnClient.UseVisualStyleBackColor = true;
            
            // 
            // RoleForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnClient);
            Controls.Add(btnModerator);
            Name = "RoleForm";
            Text = "RoleForm";
            
            ResumeLayout(false);
        }

        #endregion

        private Button btnModerator;
        private Button btnClient;
    }
}