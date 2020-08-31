using System;
using Android;
using Android.Content;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FingerAuth
{
    internal class FingerprintHandler : FingerprintManager.AuthenticationCallback
    {
        private Context mainActivity;

        public FingerprintHandler(Context mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        internal void StartAuthentication(FingerprintManager fingerprintManager, FingerprintManager.CryptoObject cryptoObject)
        {
            CancellationSignal cancellationSignal = new CancellationSignal();
            if (ActivityCompat.CheckSelfPermission(mainActivity, Manifest.Permission.UseFingerprint)
                != (int)Android.Content.PM.Permission.Granted)
                return;
            fingerprintManager.Authenticate(cryptoObject, cancellationSignal, 0, this, null);
        }
        public override void OnAuthenticationFailed()
        {
            Toast.MakeText(mainActivity, "Fingerprint Authentication failed!",ToastLength.Long).Show();
        }

        public override void OnAuthenticationSucceeded(FingerprintManager.AuthenticationResult result)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect("192.168.43.165", 3000);
                NetworkStream stream = client.GetStream(); //Gets The Stream of The Connection
                byte[] data; // creates a new byte without mentioning the size of it cuz its a byte used for sending
                data = Encoding.Default.GetBytes("1"); // put the msg in the byte ( it automaticly uses the size of the msg )
                int length = data.Length; // Gets the length of the byte data
                stream.Write(data, 0, data.Length); //Sends the real data
            }
            catch (Exception ex)
            {
            }
            mainActivity.StartActivity(new Intent(mainActivity, typeof(HomeActivity)));
        }
    }
}