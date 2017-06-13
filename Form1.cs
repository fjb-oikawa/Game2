using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game2
{
    public partial class Form1 : Form
    {
        [DllImport("user32")] static extern short GetAsyncKeyState(Keys vKey);
        List<Chara> charaList;
        TimeCounter timeCount;
        Chara ship;
        Chara[] missile;
        Chara[] enemy;
        int mtime;
        int etime;
        Random rand = new Random();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            charaList = new List<Chara>();
            timeCount = new TimeCounter();
            ship = new Chara(Properties.Resources.ship);
            missile = new Chara[5];
            for(int i = 0; i < missile.Length; i++)
            {
                missile[i] = new Chara(Properties.Resources.Missile);
                missile[i].Visible = false;
                charaList.Add(missile[i]);
            }
            enemy = new Chara[10];
            for (int i = 0; i < enemy.Length; i++)
            {
                enemy[i] = new Chara(Properties.Resources.Enemy);
                enemy[i].Visible = false;
                charaList.Add(enemy[i]);
            }

            charaList.Add(ship);
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long count = timeCount.getCount();
            for(long i = 0; i < count; i++)
            {
                //キーボード入力
                if (GetAsyncKeyState(Keys.Down) < 0)
                    ship.Y += 1;
                if (GetAsyncKeyState(Keys.Up) < 0)
                    ship.Y -= 1;
                if (GetAsyncKeyState(Keys.Right) < 0)
                    ship.X += 1;
                if (GetAsyncKeyState(Keys.Left) < 0)
                    ship.X -= 1;
                if (GetAsyncKeyState(Keys.Space) < 0 && mtime<=0)
                {
                    for(int j = 0; j < missile.Length; j++)
                    {
                        if(missile[j].Visible == false)
                        {
                            mtime = 16;
                            missile[j].X = ship.X + (48 - 16) / 2;
                            missile[j].Y = ship.Y;
                            missile[j].Visible = true;
                            break;
                        }
                    }
                }
                mtime--;
                //ミサイルの移動処理
                for (int j = 0; j < missile.Length; j++)
                {
                    missile[j].Y -= 2;
                    if (missile[j].Y < -24)
                        missile[j].Visible = false;
                }

                //敵の出現
                if(rand.Next()%100 == 0)
                {
                    for (int j = 0; j < enemy.Length; j++)
                    {
                        if(enemy[j].Visible == false)
                        {
                            enemy[j].Visible = true;
                            enemy[j].X = rand.Next() % (ClientSize.Width - 32);
                            enemy[j].Y = -32;
                            break;
                        }
                    }
                }
                //敵の移動
                for (int j = 0; j < enemy.Length; j++)
                {
                    if (enemy[j].Visible == true)
                    {
                        enemy[j].Y += 4;
                        if (enemy[j].Y > ClientSize.Height)
                            enemy[j].Visible = false;
                    }
                }
                //接触判定
                for (int j = 0; j < missile.Length; j++)
                {
                    if (missile[j].Visible == false)
                        continue;
                    for (int k = 0; k < enemy.Length; k++)
                    {
                        if (enemy[k].Visible == false)
                            continue;
                        if(enemy[k].X < missile[j].X2 && enemy[k].X2 > missile[j].X &&
                            enemy[k].Y < missile[j].Y2 && enemy[k].Y2 > missile[j].Y)
                        {
                            missile[j].Visible = false;
                            enemy[k].Visible = false;
                        }

                    }
                }

                    //リミットチェック
                    if (ship.Y < 0)
                    ship.Y = 0;
                if (ship.X < 0)
                    ship.X = 0;

                if (ship.Y > ClientSize.Height-48)
                    ship.Y = ClientSize.Height-48;

                if (ship.X > ClientSize.Width - 48)
                    ship.X = ClientSize.Width - 48;

            }
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach(Chara chara in charaList)
            {
                chara.draw(g);
            }
        }
    }
}
