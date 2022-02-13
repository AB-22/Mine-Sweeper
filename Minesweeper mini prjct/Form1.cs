using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper_mini_prjct
{
    public partial class Form1 : Form
    {
        Timer timer = new Timer();
       

        bool test = false;
       // bool _restr = false;
      //  int counter_up = 0;
        int limit =200;
        int counter_down;

        cell[][] grid; int cols; int rows; int width; public int totalmines = 20; //public int restmines = 20;
        
        public Form1()
        {
            

            InitializeComponent();
            setup();
            //timer
            timer = new Timer();
           // counter_up = 0;
            counter_down = limit;

            timer.Interval = (1000);
            timer.Tick += new EventHandler(timer1_Tick); // timer_Tick is function
            timer.Start();
           
        } 
      
        class cell
        {
         public int i;
        public int j;
          public int width;
          public  int x;
          public int y;
          public   bool mine = false;
          public bool _dismantled = false;
          public  bool revealed = false;
       public bool opened = false;
          public  int neighbor_count = 0;

           public  cell(int k, int l, int w)
            {
                i = k;
                j = l;
                width = w;
                 x = i * w;
                 y = j * w;
                
            }
          
           public bool Dismantled
           {
               get { return (this._dismantled); }
           }

          
            
         public  void count_neighbors(cell [][]grid)
           {
               if (this.mine) { this.neighbor_count = -1; }
               else
               {
                   int total = 0;

                   for (int i = -1; i <= 1; i++)
                       for (int j = -1; j <= 1; j++)
                       {

                           cell neighbor = grid[this.i + i][this.j + j];
                           if (neighbor.mine)
                           {
                               total++;



                           }
                       }

                   this.neighbor_count = total;
               }
           }
        public void show(Panel p)
         {Brush brush=new SolidBrush(Color.LightGray); 
                  Brush brush2=new SolidBrush(Color.DarkRed);     
             Pen blackPen = new Pen(Color.Black);
             Pen redPen = new Pen(Color.DarkRed);
            Font font = new Font("Times New Roman", 12.0f);
             Graphics panel_grs = p.CreateGraphics();
         StringFormat format1 = new StringFormat(StringFormatFlags.NoClip);
            
            RectangleF r=new RectangleF(this.x, this.y, this.width, this.width);
             panel_grs.DrawRectangle(blackPen, this.x, this.y, this.width, this.width);
             if (this.revealed)
             {
                 if (this.mine)
                 {
                     panel_grs.DrawLine(redPen, this.x, this.y, this.x + this.width, this.y + this.width);
                     panel_grs.DrawLine(redPen, this.x+this.width, this.y, this.x, this.y + this.width);
                 }
                 else
                 {
                     panel_grs.FillRectangle(brush, this.x + 1, this.y + 1, this.width - 1, this.width - 1);
                    if(this.neighbor_count!=0) panel_grs.DrawString(this.neighbor_count.ToString(), font, brush2, r, format1);
                 }
                 this.opened = true;
                 
             }
         }
        public void reveal() { this.revealed = true; }
         public bool contain(int x, int y)
         {bool b=(x > this.x && x < this.x + this.width && y > this.y && y < this.y + this.width);
 
           return b;

         }
       
        }

        
      
         cell[][] make2DArray(int cols, int rows)
        {
            cell[][] arr = new cell[cols+2][];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = new cell[rows+2];
            return arr;
        }

         int num_of_mined_cells(cell[][] a)
         { int num=0;
         for (int i = 0; i < cols + 2; i++)
             for (int j = 0; j < rows + 2; j++)
                 if (a[i][j].mine) num++;
         return num;
         }
         int num_of_dismanted_cells(cell[][] a)
         {
             int num = 0;
             for (int i = 0; i < cols + 2; i++)
                 for (int j = 0; j < rows + 2; j++)
                     if (a[i][j]._dismantled) num++;
             return num;
         }
         void setup()
         {
            cols = 10;
            rows = 10;
            width =panel1.Width /11;
            grid = make2DArray(cols, rows);
            for (int i = 0; i < cols+2; i++)
                for (int j = 0; j < rows + 2; j++)
                  try   {
                    grid[i][j] = new cell(i, j, width);
                    if (i == 0 || j == 0 || j == rows + 1 || i == cols + 1) {
                        grid[i][j].mine = false;
                    }
                  }
                catch(Exception e){MessageBox.Show("width is negative or zero");}
                
            Random r1 = new Random();
            int s; int d; int k = 0;
            try
            {
                while (k < totalmines)
                {

                    s = r1.Next(1, cols);
                    d = r1.Next(1, rows);
                    grid[s][d].mine = true;
                    k = num_of_mined_cells(grid);
                }
            }
            catch (Exception e) { MessageBox.Show("total number of mimes are negative"); }
            for (int i = 1; i < cols + 1; i++)
                for (int j = 1; j < rows + 1; j++)
                    grid[i][j].count_neighbors(grid);

            label1.Text = totalmines.ToString();
             
       

            
        }
               
        void wider_view(cell a)
        {

           
            if (a.i < cols+1 && a.j < rows+1 &&a.i>0&&a.j>0&&a.opened&&a.neighbor_count==0)
            {
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i != 0 || j != 0)
                        {
                            cell neighbor = grid[a.i + i][a.j + j];
                            if (neighbor.i < cols+1 && neighbor.i > 0 && neighbor.j < rows+1 && neighbor.j > 0&&!neighbor.opened)
                            {
                                neighbor.reveal();
                                neighbor.show(panel1);
                                if (neighbor.neighbor_count == 0)
                                {

                                    wider_view(neighbor);
                                

                                }
                            }
                        }
                     }
            }
            

         }
        
        public void panel1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 1; i < cols+1; i++)
                for (int j = 1; j < rows+1; j++)
                    grid[i][j].show(panel1);

                   
        }
        public void Loose()
        {
            foreach (cell[] m in grid)
                foreach (cell c in m)
                    if (c.mine)
                    {
                        c.reveal();
                        c.show(panel1);

                    }
            MessageBox.Show("YOU LOOSE");
            panel1.Enabled = false;
            timer.Stop();
            timer1.Stop();
            label2.Dispose();
            
             Application.Restart();
        }
        public void Win()
            {
                int k = 0,N=0;
            foreach (cell[] m in grid)
                foreach (cell c in m)
                    if (c.opened&&!c.mine)
                    {
                        k++;

                    }
            foreach (cell[] m in grid)
                foreach (cell c in m)
                    if (c.Dismantled && c.mine)
                    {
                        N++;

                    }
            if (N == 20 && k == 80&&counter_down!=0)
            {
                MessageBox.Show("YOU WIN");
                panel1.Enabled = false;
                timer.Stop();
                timer1.Stop();
                label2.Dispose();

                Application.Restart();
            }
 

        }

       
       
        private void mouse_Pressed(object sender, MouseEventArgs e)
        {
            int resdis = num_of_dismanted_cells(grid);
            save = false;
            restore = false;
            Graphics panel_grs = panel1.CreateGraphics();
            if (e.Button == MouseButtons.Right)
            {
                Brush brush1 = new SolidBrush(SystemColors.Control);
                Brush brush2 = new SolidBrush(Color.Red);
                for (int i = 1; i < cols + 1; i++)
                    for (int j = 1; j < rows + 1; j++)
                        if (grid[i][j].contain(e.X, e.Y))
                        {
                            if (grid[i][j].opened == false&&grid[i][j].Dismantled)
                            {
                                grid[i][j]._dismantled = false;
                                panel_grs.FillRectangle(brush1, grid[i][j].x + 1, grid[i][j].y + 1, grid[i][j].width - 1, grid[i][j].width - 1);
                            }
                            else if(grid[i][j].opened == false)
                            {
                                grid[i][j]._dismantled = true;
                                panel_grs.FillRectangle(brush2, grid[i][j].x + 1, grid[i][j].y + 1, grid[i][j].width - 1, grid[i][j].width - 1);

                            }
                            //int score = totalmines;
                           // int score = restmines;
                            
                           /* if (_restr)
                            {*/
                                //_restr = false;
                               int score = int.Parse(label1.Text) + resdis;
                                for (int k = 1; k < cols + 1; k++)
                                    for (int m = 1; m < rows + 1; m++)
                                    { if (grid[k][m]._dismantled) { score--; } label1.Text = score.ToString(); }
                          /*  }*/

                            /*else
                            for (int k = 1; k < cols + 1; k++)
                                for (int m = 1; m < rows + 1; m++)
                                { if (grid[k][m]._dismantled) { score--; } label1.Text = score.ToString(); }*/

                        }
                Win();
             }
            if (e.Button == MouseButtons.Left)
            {
                
                for (int i = 1; i < cols + 1; i++)
                    for (int j = 1; j < rows + 1; j++)
                        if (grid[i][j].contain(e.X, e.Y))
                        {
                            
                            if (grid[i][j].opened == false&&grid[i][j]._dismantled==false)
                            {
                                grid[i][j].reveal();
                                grid[i][j].show(panel1);
                               
                                if (grid[i][j].mine)
                                {
                                    test = true;
                                    Loose();


                                }

                                if (grid[i][j].neighbor_count == 0)
                                {

                                    wider_view(grid[i][j]);
                                }
                                Win();
                            }


                        }
            }
            
        }



     

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (counter_down == 0 || test||save||restore)
            { label2.Text = "" + counter_down.ToString(); }
            else
            {
                if (counter_down/*counter_up*/ >0/* limit*/)
                {
                    //counter_up++;

                    counter_down--;
                }
                else
                {
                    //counter_up = 0;
                    counter_down = limit;
                }


                label2.Text = "" + counter_down.ToString();
                Application.DoEvents();
                if (counter_down == 0)
                {
                    Loose();




                }
            }
            
           

        }    

        
        //public PaintEventArgs e { get; set; }
       public bool restore = false;
       public bool save = false;
        private void Save_Click(object sender, EventArgs e)
       {
           save = true;
            saveFileDialog_save.Title = "Save to File";
            saveFileDialog_save.FilterIndex = 2;
            saveFileDialog_save.RestoreDirectory = true;
            if (saveFileDialog_save.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog_save.FileName))
                    {
                        for (int i = 0; i < cols + 2; i++)
                            for (int j = 0; j < rows + 2; j++)
                                sw.WriteLine(grid[i][j].i + " " + grid[i][j].j + " " + grid[i][j].width + " "
                                    + grid[i][j].neighbor_count + " " + grid[i][j].mine + "/"
                                    + grid[i][j].opened + "/" + grid[i][j].revealed + "/" + grid[i][j]._dismantled + "/" + " ");
                        sw.WriteLine(label1.Text);
                        sw.WriteLine(label2.Text);


                    }
                }
                catch (IOException)
                { }

            }

        }

        private void Restore_Click(object sender, EventArgs e)
        {
            restore = true;
            //_restr = true;
            
            DialogResult result = openFileDialog_restore.ShowDialog(); 
            
            if (result == DialogResult.OK)//test result.
            {
                
                setup();
                panel1.Refresh();
                for (int i = 1; i < cols + 1; i++)
                    for (int j = 1; j < rows + 1; j++)
                        grid[i][j].show(panel1);
                string file = openFileDialog_restore.FileName;
                try
                {string[] lines =File.ReadAllLines(file);
                   
                        
                            for (int k = 0; k < lines.Length-2; k++)
                            { 
                               
                                    string[] data1 = lines[k].Split(' ');
                                    string[] data2 = data1[4].Split('/');
                                    int i = int.Parse(data1[0]);
                                    int j = int.Parse(data1[1]);
                                    int w = int.Parse(data1[2]);
                                    grid[i][j] = new cell(i, j, w);
                                    grid[i][j].neighbor_count = int.Parse(data1[3]);
                                    grid[i][j].mine = bool.Parse(data2[0]);
                                    grid[i][j].opened = bool.Parse(data2[1]);
                                    grid[i][j].revealed = bool.Parse(data2[2]);
                                    grid[i][j]._dismantled = bool.Parse(data2[3]);
                                    if (i != 0 && j != 0 && j != rows + 1 && i != cols + 1)
                                       grid[i][j].show(panel1);
                                    Graphics panel_grs = panel1.CreateGraphics();
                                    Brush brush2 = new SolidBrush(Color.Red);
                                    if (grid[i][j]._dismantled) panel_grs.FillRectangle(brush2, grid[i][j].x + 1, grid[i][j].y + 1, grid[i][j].width - 1, grid[i][j].width - 1);
                                   

                                
                            }
                          //  restmines =int.Parse(lines[lines.Length - 2]);
                            counter_down = int.Parse(lines[lines.Length - 1]);
                            label1.Text = lines[lines.Length - 2];

                        
                }
                catch (Exception)
                {
                    MessageBox.Show("Error in uploading file");
                }
            }
        }

       

        
    }
}
