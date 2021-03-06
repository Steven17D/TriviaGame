using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Client
{
    public partial class Menu : Form
    {
        System.Net.Sockets.TcpClient clientSocket;

        public void msg(string mesg) { MessageBox.Show(mesg); }
        public Menu(System.Net.Sockets.TcpClient clientSocket_)
        {
            InitializeComponent();
            clientSocket = clientSocket_;
        }

        private void SignIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (userNameBox.Text.Length == 0 || passwordBox.Text.Length == 0)
                {
                    msgToUser.Text = "Please fill all fields";
                    msgToUser.Visible = true;
                    return;
                }
                string signInMsg = "200" +
                userNameBox.Text.Length.ToString().PadLeft(2, '0') + userNameBox.Text +
                passwordBox.Text.Length.ToString().PadLeft(2, '0') + passwordBox.Text;
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(signInMsg);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                byte[] inStream = new byte[4];
                serverStream.Read(inStream, 0, 4);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                //msg(returndata);

                switch (returndata.Substring(0,4))
                {
                    case "1020":
                        //Success
                        Sign_up_outB.Text = "Sign out";
                        UserNameLable.Hide();
                        PasswordLable.Hide();
                        userNameBox.Hide();
                        passwordBox.Hide();
                        SignIn.Hide();
                        GreetingsLable.Text = "Hello " + userNameBox.Text;
                        GreetingsLable.Visible = true;
                        TriviaLogoTop.Image = Properties.Resources.Trivia;
                        TriviaLogoRight.Image = Properties.Resources.Trivia;
                        TriviaLogoLeft.Image = Properties.Resources.Trivia;
                        JoinRoom.Enabled = true;
                        CreateRoom.Enabled = true;
                        MyStatus.Enabled = true;
                        BestScores.Enabled = true;
                        JoinRoom.Show();
                        CreateRoom.Show();
                        MyStatus.Show();
                        BestScores.Show();
                        msgToUser.Text = "";
                        msgToUser.Visible = false;
                        break;
                    case "1021":
                        //Wrong Details
                        msgToUser.Text = "Wrong Details";
                        msgToUser.Visible = true;
                        //set trivia to red
                        TriviaLogoTop.Image = Properties.Resources.ErrorTriviaLogo;
                        TriviaLogoRight.Image = Properties.Resources.ErrorTriviaLogo;
                        TriviaLogoLeft.Image = Properties.Resources.ErrorTriviaLogo;
                        break;
                    case "1022":
                        //User is already connected
                        msgToUser.Text = "User is already connected";
                        msgToUser.Visible = true;
                        //set trivia to red
                        TriviaLogoTop.Image = Properties.Resources.ErrorTriviaLogo;
                        TriviaLogoRight.Image = Properties.Resources.ErrorTriviaLogo;
                        TriviaLogoLeft.Image = Properties.Resources.ErrorTriviaLogo;
                        break;
                    default:
                        msg("Got unknown code: " + returndata);
                        break;
                }
            }
            catch (Exception exc)
            {
                msg(exc.Message);
            }
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            string exitMsg = "299";
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(exitMsg);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            Application.Exit();
        }

        private void Sign_up_outB_Click(object sender, EventArgs e)
        {
            if (Sign_up_outB.Text == "Sign up")
            {
                this.Hide();
                Form SignUpForm = new SignUp(clientSocket,this);
                SignUpForm.Show();
            }
            else if (Sign_up_outB.Text == "Sign out")
            {
                string signOutMsg = "201";
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(signOutMsg);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                Sign_up_outB.Text = "Sign up";
                UserNameLable.Show();
                PasswordLable.Show(); 
                userNameBox.Text = "";
                passwordBox.Text = "";
                userNameBox.Show();
                passwordBox.Show();
                SignIn.Show();
                GreetingsLable.Visible = false;
                JoinRoom.Hide();
                CreateRoom.Hide();
                MyStatus.Hide();
                BestScores.Hide();
            }
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            JoinRoom.Hide();
            CreateRoom.Hide();
            MyStatus.Hide();
            BestScores.Hide();
        }

        private void JoinRoom_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form SignUpForm = new RoomSelect(clientSocket, this);
            SignUpForm.Show();
        }

        private void Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            string exitMsg = "299";
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(exitMsg);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private void MyStatus_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form myStatusForm = new MyStatus(clientSocket, this);
            myStatusForm.Show();
        }

        private void BestScores_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form myStatusForm = new BestScores(clientSocket, this);
            myStatusForm.Show();
        }

        private void CreateRoom_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form myStatusForm = new RoomCreate(clientSocket, this);
            myStatusForm.Show();
        } 
    }
}
