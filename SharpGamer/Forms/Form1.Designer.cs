namespace SharpGamer.Forms
{
    partial class Form1
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.numGenerationsPicker = new System.Windows.Forms.NumericUpDown();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.mutationRatePicker = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.crossOverRatePicker = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.maxStepSizePicker = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pcPicker = new System.Windows.Forms.NumericUpDown();
            this.networkPicker = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGenerationsPicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mutationRatePicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crossOverRatePicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxStepSizePicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcPicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.networkPicker)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox1.Location = new System.Drawing.Point(492, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(500, 500);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 74);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.start_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(663, 518);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(61, 50);
            this.button2.TabIndex = 2;
            this.button2.Text = "Test Network";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.pause_click);
            // 
            // numGenerationsPicker
            // 
            this.numGenerationsPicker.Location = new System.Drawing.Point(68, 38);
            this.numGenerationsPicker.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numGenerationsPicker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numGenerationsPicker.Name = "numGenerationsPicker";
            this.numGenerationsPicker.Size = new System.Drawing.Size(86, 20);
            this.numGenerationsPicker.TabIndex = 5;
            this.numGenerationsPicker.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // richTextBox2
            // 
            this.richTextBox2.DetectUrls = false;
            this.richTextBox2.Location = new System.Drawing.Point(249, 121);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new System.Drawing.Size(231, 447);
            this.richTextBox2.TabIndex = 7;
            this.richTextBox2.Text = "";
            // 
            // richTextBox1
            // 
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.Location = new System.Drawing.Point(12, 121);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(231, 447);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 92);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(468, 23);
            this.progressBar1.TabIndex = 9;
            // 
            // mutationRatePicker
            // 
            this.mutationRatePicker.DecimalPlaces = 2;
            this.mutationRatePicker.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.mutationRatePicker.Location = new System.Drawing.Point(68, 66);
            this.mutationRatePicker.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mutationRatePicker.Name = "mutationRatePicker";
            this.mutationRatePicker.Size = new System.Drawing.Size(86, 20);
            this.mutationRatePicker.TabIndex = 10;
            this.mutationRatePicker.Value = new decimal(new int[] {
            35,
            0,
            0,
            131072});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(160, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Num Generations";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Mutation Rate";
            // 
            // crossOverRatePicker
            // 
            this.crossOverRatePicker.DecimalPlaces = 2;
            this.crossOverRatePicker.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.crossOverRatePicker.Location = new System.Drawing.Point(259, 38);
            this.crossOverRatePicker.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.crossOverRatePicker.Name = "crossOverRatePicker";
            this.crossOverRatePicker.Size = new System.Drawing.Size(86, 20);
            this.crossOverRatePicker.TabIndex = 13;
            this.crossOverRatePicker.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(351, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Crossover Rate";
            // 
            // maxStepSizePicker
            // 
            this.maxStepSizePicker.DecimalPlaces = 2;
            this.maxStepSizePicker.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.maxStepSizePicker.Location = new System.Drawing.Point(259, 66);
            this.maxStepSizePicker.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxStepSizePicker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            327680});
            this.maxStepSizePicker.Name = "maxStepSizePicker";
            this.maxStepSizePicker.Size = new System.Drawing.Size(86, 20);
            this.maxStepSizePicker.TabIndex = 15;
            this.maxStepSizePicker.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(351, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Max Step Size";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(160, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Pc";
            // 
            // pcPicker
            // 
            this.pcPicker.DecimalPlaces = 2;
            this.pcPicker.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.pcPicker.Location = new System.Drawing.Point(68, 12);
            this.pcPicker.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            131072});
            this.pcPicker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.pcPicker.Name = "pcPicker";
            this.pcPicker.Size = new System.Drawing.Size(86, 20);
            this.pcPicker.TabIndex = 19;
            this.pcPicker.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // networkPicker
            // 
            this.networkPicker.Location = new System.Drawing.Point(730, 535);
            this.networkPicker.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.networkPicker.Name = "networkPicker";
            this.networkPicker.Size = new System.Drawing.Size(120, 20);
            this.networkPicker.TabIndex = 20;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 576);
            this.Controls.Add(this.networkPicker);
            this.Controls.Add(this.pcPicker);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.maxStepSizePicker);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.crossOverRatePicker);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mutationRatePicker);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.numGenerationsPicker);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGenerationsPicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mutationRatePicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crossOverRatePicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxStepSizePicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcPicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.networkPicker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.NumericUpDown numGenerationsPicker;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.NumericUpDown mutationRatePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown crossOverRatePicker;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown maxStepSizePicker;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown pcPicker;
        private System.Windows.Forms.NumericUpDown networkPicker;
    }
}