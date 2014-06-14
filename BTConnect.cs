using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Windows.Forms;
using SharpAccessory.GenericBusinessClient.Plugging;
using SharpAccessory.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using VMscope.VMSlideExplorer;
using VMscope.VMSlideExplorer.VisualComponents;

namespace TestPlugin
{
  public class BTConnect : Plugin
  {
    readonly Guid OurServiceClassId = new Guid("{29913A2D-EB93-40cf-BBB8-DEEE26452197}");
    readonly string OurServiceName = "32feet.NET Chat2";

    private WsiToolButton wtbConnect;
    private Microscope microscope;
    private TextBox tbListing;
    private TextBox textInput;

    private string receivedData = "";
    public string ReceivedData
    {
      get { return receivedData; }
      set
      {
        if (value != receivedData)
        {
          receivedData = value;
          
        }
      }
    }

    volatile bool _closing;
    TextWriter _connWtr;
    BluetoothListener _lsnr;

    protected override void OnBroadcastContext(BroadcastContextEventArgs e)
    {
      base.OnBroadcastContext(e);

      if (e.Context is Microscope)
      {
        microscope = e.Context as Microscope;
        microscope.WsiCompositesChanged += OnWsiCompositesChanged;

        wtbConnect = microscope.ToolBar.CreateToolButton();
        wtbConnect.Image = TangoIconSet.LoadIcon(TangoIcon.Input_Gaming);
        wtbConnect.ToolTipText = "Disconnect";
        wtbConnect.Click += new EventHandler(wtbDisconnect_Click);

        wtbConnect = microscope.ToolBar.CreateToolButton();
        wtbConnect.Image = TangoIconSet.LoadIcon(TangoIcon.Input_Gaming);
        wtbConnect.ToolTipText = "Mode Offline";
        wtbConnect.Click += new EventHandler(wtbModeNeither_Click);

        wtbConnect = microscope.ToolBar.CreateToolButton();
        wtbConnect.Image = TangoIconSet.LoadIcon(TangoIcon.Input_Gaming);
        wtbConnect.ToolTipText = "Mode Connectable";
        wtbConnect.Click += new EventHandler(wtbModeConnectable_Click);

        wtbConnect = microscope.ToolBar.CreateToolButton();
        wtbConnect.Image = TangoIconSet.LoadIcon(TangoIcon.Input_Gaming);
        wtbConnect.ToolTipText = "Mode Discoverable";
        wtbConnect.Click += new EventHandler(wtbModeDiscoverable_Click);

        wtbConnect = microscope.ToolBar.CreateToolButton();
        wtbConnect.Image = TangoIconSet.LoadIcon(TangoIcon.Input_Gaming);
        wtbConnect.ToolTipText = "Show Radio Information";
        wtbConnect.Click += new EventHandler(wtbShowRadioInfo_Click);

        wtbConnect = microscope.ToolBar.CreateToolButton();
        wtbConnect.Image = TangoIconSet.LoadIcon(TangoIcon.Input_Gaming);
        wtbConnect.ToolTipText = "Connect by Address";
        wtbConnect.Click += new EventHandler(wtbConnectByAddress_Click);

        wtbConnect = microscope.ToolBar.CreateToolButton();
        wtbConnect.Image = TangoIconSet.LoadIcon(TangoIcon.Input_Gaming);
        wtbConnect.ToolTipText = "Connect by Select";
        wtbConnect.Click += new EventHandler(wtbConnectBySelect_Click);

        textInput = new TextBox();
        textInput.Parent = microscope.DockAreas.Bottom;
        textInput.Dock = DockStyle.Bottom;
        textInput.BackColor = Color.Yellow;
        textInput.WordWrap = false;
        textInput.Visible = true;
        textInput.KeyPress += new KeyPressEventHandler(textBoxInput_KeyPress);

        tbListing = new TextBox();
        tbListing.Parent = microscope.DockAreas.Bottom;
        tbListing.ScrollBars = ScrollBars.Both;
        tbListing.Dock = DockStyle.Bottom;
        tbListing.BackColor = Color.Red;
        tbListing.WordWrap = false;
        tbListing.Multiline = true;
        tbListing.Visible = true;
        tbListing.Height = 100;

        StartBluetooth();
        AddMessage(MessageSource.Info,
          "Connect to another remote device running the app."
          + "  Each person can then enter text in the box at the bottom"
          + " and hit return to send it."
          + "  Of course the radio on the target device will have to be"
          + " in connectable and/or discoverable mode.");
        //Unselect the text.
        tbListing.Select(0, 0);
        // Focus to the input-box.
        this.textInput.Select();
      }
    }

    List<BTHandler> composites = new List<BTHandler>();

    private void OnWsiCompositesChanged(object sender, EventArgs e)
    {
      composites.Clear();
      for (int i = 0; i < microscope.WsiComposites.Count; i++)
      {
        WsiComposite composite = microscope.WsiComposites[i];
        composites.Add(new BTHandler(composite));
      }
    }

    private void translateView(System.Drawing.PointF p)
    {
      foreach (var item in composites)
      {
        item.TranslateView(p);
      }
    }

    private System.Drawing.PointF LastPosition;

    /*
     * Command Vector Point 
     * C:UP 
     * V:12.345678;43.212313 
     * P:12.345678;43.213123
     * */
   
    private void handleInput(string input)
    {
      string[] part = input.Split(new char[]{':'}, StringSplitOptions.RemoveEmptyEntries);
      switch (part[0])
      {
        case "T": // Text
          AddMessage(MessageSource.Remote, input);
          break;
        case "P": // Point
          AddMessage(MessageSource.Remote, input);
          break;
        case "V": // Vector
          string[] value = part[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
          //input = input.Replace(",", ".");
          //input = input.Replace(";", ",");
          System.Drawing.PointF ppp = new System.Drawing.PointF(
            float.Parse(value[0], CultureInfo.InvariantCulture),
            float.Parse(value[1], CultureInfo.InvariantCulture)
          );
          //System.Windows.Point p = System.Windows.Point.Parse(input);
          translateView(ppp);
          LastPosition = ppp;
          AddMessage(MessageSource.Local, ppp.ToString());
          break;
        default:
          AddMessage(MessageSource.Remote, input);
          break;
      }
    }

    #region Bluetooth start/Connect/Listen
    private void StartBluetooth()
    {
      try
      {
        new BluetoothClient();
      }
      catch (Exception ex)
      {
        var msg = "Bluetooth init failed: " + ex;
        MessageBox.Show(msg);
        throw new InvalidOperationException(msg, ex);
      }
      // TODO Check radio?
      //
      // Always run server?
      StartListener();
    }

    BluetoothAddress BluetoothSelect()
    {
      var dlg = new SelectBluetoothDeviceDialog();
      var rslt = dlg.ShowDialog();
      if (rslt != DialogResult.OK)
      {
        AddMessage(MessageSource.Info, "Cancelled select device.");
        return null;
      }
      var addr = dlg.SelectedDevice.DeviceAddress;
      return addr;
    }

    void BluetoothConnect(BluetoothAddress addr)
    {
      var cli = new BluetoothClient();
      try
      {
        cli.Connect(addr, OurServiceClassId);
        var peer = cli.GetStream();
        SetConnection(peer, true, cli.RemoteEndPoint);
        ThreadPool.QueueUserWorkItem(ReadMessagesToEnd_Runner, peer);
      }
      catch (SocketException ex)
      {
        // Try to give a explanation reason by checking what error-code.
        // http://32feet.codeplex.com/wikipage?title=Errors
        // Note the error codes used on MSFT+WM are not the same as on
        // MSFT+Win32 so don't expect much there, we try to use the
        // same error codes on the other platforms where possible.
        // e.g. Widcomm doesn't match well, Bluetopia does.
        // http://32feet.codeplex.com/wikipage?title=Feature%20support%20table
        string reason;
        switch (ex.ErrorCode)
        {
          case 10048: // SocketError.AddressAlreadyInUse
            // RFCOMM only allow _one_ connection to a remote service from each device.
            reason = "There is an existing connection to the remote Chat2 Service";
            break;
          case 10049: // SocketError.AddressNotAvailable
            reason = "Chat2 Service not running on remote device";
            break;
          case 10064: // SocketError.HostDown
            reason = "Chat2 Service not using RFCOMM (huh!!!)";
            break;
          case 10013: // SocketError.AccessDenied:
            reason = "Authentication required";
            break;
          case 10060: // SocketError.TimedOut:
            reason = "Timed-out";
            break;
          default:
            reason = null;
            break;
        }
        reason += " (" + ex.ErrorCode.ToString() + ") -- ";
        //
        var msg = "Bluetooth connection failed: " + MakeExceptionMessage(ex);
        msg = reason + msg;
        AddMessage(MessageSource.Error, msg);
        MessageBox.Show(msg);
      }
      catch (Exception ex)
      {
        var msg = "Bluetooth connection failed: " + MakeExceptionMessage(ex);
        AddMessage(MessageSource.Error, msg);
        MessageBox.Show(msg);
      }
    }

    private void StartListener()
    {
      var lsnr = new BluetoothListener(OurServiceClassId);
      lsnr.ServiceName = OurServiceName;
      lsnr.Start();
      _lsnr = lsnr;
      ThreadPool.QueueUserWorkItem(ListenerAccept_Runner, lsnr);
    }

    void ListenerAccept_Runner(object state)
    {
      var lsnr = (BluetoothListener)_lsnr;
      // We will accept only one incoming connection at a time. So just
      // accept the connection and loop until it closes.
      // To handle multiple connections we would need one threads for
      // each or async code.
      while (true)
      {
        var conn = lsnr.AcceptBluetoothClient();
        var peer = conn.GetStream();
        SetConnection(peer, false, conn.RemoteEndPoint);
        ReadMessagesToEnd(peer);
      }
    }
    #endregion

    #region Connection Set/Close
    private void SetConnection(Stream peerStream, bool outbound, BluetoothEndPoint remoteEndPoint)
    {
      if (_connWtr != null)
      {
        AddMessage(MessageSource.Error, "Already Connected!");
        return;
      }
      _closing = false;
      var connWtr = new StreamWriter(peerStream);
      connWtr.NewLine = "\r\n"; // Want CR+LF even on UNIX/Mac etc.
      _connWtr = connWtr;
      ClearScreen();
      AddMessage(MessageSource.Info,
          (outbound ? "Connected to " : "Connection from ")
        // Can't guarantee that the Port is set, so just print the address.
        // For more info see the docs on BluetoothClient.RemoteEndPoint.
          + remoteEndPoint.Address);
    }

    private void ConnectionCleanup()
    {
      _closing = true;
      var wtr = _connWtr;
      //_connStrm = null;
      _connWtr = null;
      if (wtr != null)
      {
        try
        {
          wtr.Close();
        }
        catch (Exception ex)
        {
          Debug.WriteLine("ConnectionCleanup close ex: " + MakeExceptionMessage(ex));
        }
      }
    }

    void BluetoothDisconnect()
    {
      AddMessage(MessageSource.Info, "Disconnecting");
      ConnectionCleanup();
    }
    #endregion

    #region Connection I/O
    private bool Send(string message)
    {
      if (_connWtr == null)
      {
        MessageBox.Show("No connection.");
        return false;
      }
      try
      {
        _connWtr.WriteLine(message);
        _connWtr.Flush();
        return true;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Connection lost! (" + MakeExceptionMessage(ex) + ")");
        ConnectionCleanup();
        return false;
      }
    }

    private void ReadMessagesToEnd_Runner(object state)
    {
      Stream peer = (Stream)state;
      ReadMessagesToEnd(peer);
    }

    private void ReadMessagesToEnd(Stream peer)
    {
      var rdr = new StreamReader(peer);
      while (true)
      {
        string line;
        try
        {
          line = rdr.ReadLine();
        }
        catch (IOException ioex)
        {
          if (_closing)
          {
            // Ignore the error that occurs when we're in a Read
            // and _we_ close the connection.
          }
          else
          {
            AddMessage(MessageSource.Error, "Connection was closed hard (read).  "
                + MakeExceptionMessage(ioex));
          }
          break;
        }
        if (line == null)
        {
          AddMessage(MessageSource.Info, "Connection was closed (read).");
          break;
        }
        //AddMessage(MessageSource.Remote, line);
        handleInput(line);
      }//while
      ConnectionCleanup();
    }
    #endregion

    #region Radio
    void SetRadioMode(RadioMode mode)
    {
      try
      {
        BluetoothRadio.PrimaryRadio.Mode = mode;
      }
      catch (NotSupportedException)
      {
        MessageBox.Show("Setting Radio.Mode not supported on this Bluetooth stack.");
      }
    }

    static void DisplayPrimaryBluetoothRadio(TextWriter wtr)
    {
      var myRadio = BluetoothRadio.PrimaryRadio;
      if (myRadio == null)
      {
        wtr.WriteLine("No radio hardware or unsupported software stack");
        return;
      }
      var mode = myRadio.Mode;
      // Warning: LocalAddress is null if the radio is powered-off.
      wtr.WriteLine("* Radio, address: {0:C}", myRadio.LocalAddress);
      wtr.WriteLine("Mode: " + mode.ToString());
      wtr.WriteLine("Name: " + myRadio.Name);
      wtr.WriteLine("HCI Version: " + myRadio.HciVersion
          + ", Revision: " + myRadio.HciRevision);
      wtr.WriteLine("LMP Version: " + myRadio.LmpVersion
          + ", Subversion: " + myRadio.LmpSubversion);
      wtr.WriteLine("ClassOfDevice: " + myRadio.ClassOfDevice
          + ", device: " + myRadio.ClassOfDevice.Device
          + " / service: " + myRadio.ClassOfDevice.Service);
      wtr.WriteLine("S/W Manuf: " + myRadio.SoftwareManufacturer);
      wtr.WriteLine("H/W Manuf: " + myRadio.Manufacturer);
    }
    #endregion

    #region Menu items etc
    private void wtbConnectBySelect_Click(object sender, EventArgs e)
    {
      var addr = BluetoothSelect();
      if (addr == null)
      {
        return;
      }
      BluetoothConnect(addr);
    }

    private void wtbConnectByAddress_Click(object sender, EventArgs e)
    {
      var addr = BluetoothAddress.Parse("002233445566");
      var line = Microsoft.VisualBasic.Interaction.InputBox("Target Address", "Chat2", null, -1, -1);
      if (string.IsNullOrEmpty(line))
      {
        return;
      }
      line = line.Trim();
      if (!BluetoothAddress.TryParse(line, out addr))
      {
        MessageBox.Show("Invalid address.");
        return;
      }
      BluetoothConnect(addr);
    }

    private void wtbDisconnect_Click(object sender, EventArgs e)
    {
      BluetoothDisconnect();
    }

    private void textBoxInput_KeyPress(object sender, KeyPressEventArgs e)
    {
      var cr = e.KeyChar == '\r';
      var lf = e.KeyChar == '\n';
      if (cr || lf)
      {
        e.Handled = true;
        SendMessage();
      }
    }

    private void SendMessage()
    {
      var message = this.textInput.Text;
      bool successSend = Send(message);
      if (successSend)
      {
        AddMessage(MessageSource.Local, message);
        this.textInput.Text = string.Empty;
      }
    }

    //--
    private void wtbModeDiscoverable_Click(object sender, EventArgs e)
    {
      SetRadioMode(RadioMode.Discoverable);
    }

    private void wtbModeConnectable_Click(object sender, EventArgs e)
    {
      SetRadioMode(RadioMode.Connectable);
    }

    private void wtbModeNeither_Click(object sender, EventArgs e)
    {
      SetRadioMode(RadioMode.PowerOff);
    }

    private void wtbShowRadioInfo_Click(object sender, EventArgs e)
    {
      using (var wtr = new StringWriter())
      {
        DisplayPrimaryBluetoothRadio(wtr);
        AddMessage(MessageSource.Info, wtr.ToString());
      }
    }
    #endregion

    #region Chat Log
    private void ClearScreen()
    {
      EventHandler action = delegate
      {
        AssertOnUiThread();
        this.tbListing.Text = string.Empty;
      };
      ThreadSafeRun(action);
    }

    enum MessageSource
    {
      Local,
      Remote,
      Info,
      Error,
    }

    void AddMessage(MessageSource source, string message)
    {
      EventHandler action = delegate
      {
        string prefix;
        switch (source)
        {
          case MessageSource.Local:
            prefix = "Me: ";
            break;
          case MessageSource.Remote:
            prefix = "You: ";
            break;
          case MessageSource.Info:
            prefix = "Info: ";
            break;
          case MessageSource.Error:
            prefix = "Error: ";
            break;
          default:
            prefix = "???:";
            break;
        }
        AssertOnUiThread();
        this.tbListing.Text =
            prefix + message + "\r\n"
            + this.tbListing.Text;
      };
      ThreadSafeRun(action);
    }

    private void ThreadSafeRun(EventHandler action)
    {
      Control c = this.tbListing;
      if (c.InvokeRequired)
      {
        c.BeginInvoke(action);
      }
      else
      {
        action(null, null);
      }
    }
    #endregion

    private static string MakeExceptionMessage(Exception ex)
    {
      return ex.Message;
    }

    private void AssertOnUiThread()
    {
      Debug.Assert(!this.tbListing.InvokeRequired, "UI access from non UI thread!");
    }


  }

}
