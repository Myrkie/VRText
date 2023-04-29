﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using VRText.SharpOSC;
using VRText.Config;
using VRText.Utils;
using VRText.Spotify;
using System.Threading;
using System.Net.Sockets;
using VRText.Handlers;
using OscMessage = VRText.SharpOSC.OscMessage;
using UDPSender = VRText.SharpOSC.UDPSender;

namespace VRText.src.VRText.UI
{
    public partial class Settings : Form
    {
        private MainForm mainForm;
        public Settings(MainForm parent)
        {
            this.mainForm = parent;
            InitializeComponent();
            InitializeSettings();
            configControls();
        }

        private void InitializeSettings()
        {
            var loadSettings = SQLiteHandler.LoadSettings();
            string serverAddress;
            string serverPort;
            string spotifyPrefix;
            string lang;
            
            serverAddress = OSC.getAddress();
            serverPort = OSC.getAddressPort().ToString();
            spotifyPrefix = SpotifyHandler.getPrefix();
            lang = mainForm.language;

            if (loadSettings.Any())
            {
                var settingsValues = loadSettings[0];
                
                serverAddress = settingsValues[0];
                serverPort = settingsValues[1];
                spotifyPrefix = settingsValues[2];
                lang = settingsValues[3];
                
                var language = new Lang(lang).GetCurrentLanguage();
                if (language != null)
                {
                    this.mainForm.lang = language;
                }
                
                this.mainForm.SetComponentLanguage(mainForm);
                this.mainForm.SetComponentLanguage(this);
            }

            serverAddressInput.Text = serverAddress;
            portInput.Text = serverPort;
            SpotifyPrefixInput.Text = spotifyPrefix;

            string[] data = { serverAddress, serverPort, spotifyPrefix, lang };
            
            if (!loadSettings.Any()) SQLiteHandler.InitSettings(data);

        }

        private void languageOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lang = languageOptions.Text;
            string variant;

            switch(lang)
            {
                case "English":
                    variant = "en-US";
                    break;
                case "Português do Brasil":
                    variant = "pt-BR";
                    break;
                case "Español":
                    variant = "es-419";
                    break;
                case "Deutsch":
                    variant = "de-DE";
                    break;
                case "Italiano":
                    variant = "it-IT";
                    break;
                case "Français":
                    variant = "fr-FR";
                    break;
                case "Norsk":
                    variant = "no-NO";
                    break;
                default:
                    MessageBox.Show("Invalid language or incomplete\nreverting to default");
                    variant = "en-US";
                    break;
            }
            
            var language = new Lang(variant).GetCurrentLanguage();
            if (language != null)
            {
                this.mainForm.lang = language;
            }
            this.mainForm.SetComponentLanguage(mainForm);
            this.mainForm.SetComponentLanguage(this);
            this.mainForm.language = variant;
            SQLiteHandler.UpdateLanguageSettings(variant);
        }

        private void configControls()
        {
            var currentLanguage = this.mainForm.language;
            Console.WriteLine(currentLanguage);

            switch (currentLanguage)
            {
                case "en-US":
                    languageOptions.SelectedItem = "English";

                    return;
                case "pt-BR":
                    languageOptions.SelectedItem = "Português do Brasil";

                    return;
                case "es-419":
                    languageOptions.SelectedItem = "Español";
                    return;
                case "de-DE":
                    languageOptions.SelectedItem = "Deutsch";

                    return;
                case "fr-FR":
                    languageOptions.SelectedItem = "Français";

                    return;
                case "no-NO":
                    languageOptions.SelectedItem = "Norsk";

                    return;
                case "it-IT":
                    languageOptions.SelectedItem = "Italiano";
                    break;
            }
        }

        private void testConnectionButton_Click(object sender, EventArgs e)
        {
            var address = serverAddressInput.Text;
            var port = int.Parse(portInput.Text);
            var delay = new Interval();
            var inputMessage = new OscMessage(OSC.getTypingEndPoint(), true);
            var sendMessage = new UDPSender(address, port);

            sendMessage.Send(inputMessage);
            testLabel.Visible = true;
            testLabel.Text = "Test sent over: " + address + ":" + port;
            OSC.setNewAddress(address, portInput.Text);
            delay.setTimeout(() =>
            {
                inputMessage = new OscMessage(OSC.getTypingEndPoint(), false);
                sendMessage.Send(inputMessage);
            }, 2000);
        }

        private void ResetSettings_Click(object sender, EventArgs e)
        {
            var serverAddress = "127.0.0.1";
            var serverPort = "9000";

            OSC.setNewAddress(serverAddress, serverPort);
            serverAddressInput.Text = serverAddress;
            portInput.Text = serverPort;
            testLabel.Text = "Default settings have been set.";

        }

        private void SpotifyPrefixInput_KeyUp(object sender, KeyEventArgs e)
        {
            string prefix = SpotifyPrefixInput.Text;
            SpotifyHandler.setPrefix(prefix);
            SQLiteHandler.UpdateSpotifyPrefix(prefix);
        }

        private void GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/LeadsBuilds");
        }
    }
}
