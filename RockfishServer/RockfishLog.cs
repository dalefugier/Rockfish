using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using RockfishCommon;

namespace RockfishServer
{
  /// <summary>
  /// RockfishLog class (singleton)
  /// </summary>
  public class RockfishLog
  {
    private readonly object m_locker;
    private readonly Queue<RockfishHeader> m_queue;
    private readonly Timer m_timer;
    private string m_path;

    /// <summary>
    /// The type of logging 
    /// </summary>
    public enum LogType
    {
      /// <summary>
      /// Logging is disabled
      /// </summary>
      Disabled,
      /// <summary>
      /// Log events, rotate the log file daily
      /// </summary>
      Daily,
      /// <summary>
      /// Log events, rotate the log file weekly
      /// </summary>
      Weekly,
      /// <summary>
      /// Log events, rotate the log file monthly
      /// </summary>
      Monthly
    }

    /// <summary>
    /// Private constructor
    /// </summary>
    private RockfishLog()
    {
      m_locker = new object();
      m_queue = new Queue<RockfishHeader>();
      m_timer = new Timer { Interval = 5000 };
      m_timer.Elapsed += OnTimerElapsed;
    }

    /// <summary>
    /// The one and only RockfishLog object.
    /// </summary>
    static RockfishLog g_the_log;

    /// <summary>
    /// Returns the one and only RockfishLog object
    /// </summary>
    public static RockfishLog TheLog => g_the_log ?? (g_the_log = new RockfishLog());

    /// <summary>
    /// Starts the timer that flushes the log queue.
    /// </summary>
    public bool Start()
    {
      if (LogType.Disabled != RockfishServerPlugIn.LogType())
      {
        m_timer.Start();
        return true;
      }
      return false;
    }

    /// <summary>
    /// Stops the timer that flushes the log queue.
    /// </summary>
    public void Stop()
    {
      m_timer.Stop();
    }

    /// <summary>
    /// Triggered by timer
    /// </summary>
    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
      Write();
    }

    /// <summary>
    /// Adds an item to the queue.
    /// </summary>
    public void Enqueue(RockfishHeader header)
    {
      if (null != header)
      {
        lock (m_locker)
        {
          if (RockfishLog.LogType.Disabled != RockfishServerPlugIn.LogType())
            m_queue.Enqueue(header);
        }
      }
    }

    /// <summary>
    /// Writes the records to disk.
    /// </summary>
    public bool Write()
    {
      var path = LogPath;
      if (string.IsNullOrEmpty(path))
        return false;

      var rc = false;
      lock (m_locker)
      {
        if (m_queue.Count > 0)
        {
          try
          {
            var exists = File.Exists(path);
            using (var writer = new StreamWriter(path, true))
            {
              if (!exists)
                writer.WriteLine(RockfishHeader.ToHeading());

              foreach (var header in m_queue)
                writer.WriteLine(header.ToString());

              writer.Flush();
              writer.Close();

              m_queue.Clear();

              rc = true;
            }
          }
          catch
          {
            // ignored
          }
        }
      }

      return rc;
    }

    /// <summary>
    /// Returns the full path to the log file.
    /// </summary>
    private string LogPath
    {
      get
      {
        var path = LogFileFolder;
        if (string.IsNullOrEmpty(path))
          return null;

        path += Path.DirectorySeparatorChar;

        const string ext = "csv";
        var local_time = DateTime.Now;
        string filename;

        switch (RockfishServerPlugIn.LogType())
        {
          case LogType.Daily:
            filename = $"{path}{local_time:yyyyMMdd}.{ext}";
            break;

          case LogType.Weekly:
            {
              var week_day = (int)local_time.DayOfWeek;
              if (week_day > 0)
              {
                var time_span = new TimeSpan(week_day, 0, 0, 0);
                local_time -= time_span;
              }
            }
            filename = $"{path}{local_time:yyyyMMdd}.{ext}";
            break;

          case LogType.Monthly:
            filename = $"{path}{local_time:yyyyMM}01.{ext}";
            break;

          default:
            return null;
        }

        return filename;
      }
    }

    /// <summary>
    /// Returns the log file folder.
    /// </summary>
    private string LogFileFolder
    {
      get
      {
        if (string.IsNullOrEmpty(m_path))
        {
          var sep = Path.DirectorySeparatorChar;
          var sb = new StringBuilder(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
          sb.Append(sep);
          sb.Append(RockfishServerPlugIn.ThePlugIn.Name);
          sb.Append(sep);
          sb.Append(RockfishServerPlugIn.ThePlugIn.Version);
          sb.Append(sep);
          sb.Append("Logs");
          m_path = sb.ToString();
        }

        if (!Directory.Exists(m_path))
        {
          try
          {
            Directory.CreateDirectory(m_path);
          }
          catch
          {
            // ignored
          }
        }

        return !Directory.Exists(m_path) ? null : m_path;
      }
    }

  }
}
