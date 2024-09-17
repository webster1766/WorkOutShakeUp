using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WosiData;
using WosiDomain;
using WosiDomain.MongoDocs;

namespace WosuUi
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    //private readonly ConnectedData _data = new ConnectedData();
    private readonly MongoData _data = new MongoData();
    private MovementDoc _currentMovement;
    private bool _isLoading;
    private bool _isEditing;
    private CollectionViewSource _movementViewSource;

    private const int MOVEMENT_COL_INDEX = 0;
    private const int NOTES_COL_INDEX = 1;

    public MainWindow()
    {
      InitializeComponent();
      this.BodyPartToAdd.Text = string.Empty;
      this.EquipToAdd.Text = string.Empty;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      this.Refresh();
    }
    private void movementDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
      var _currentMovement = (MovementDoc)this.movementDataGrid.SelectedItem;
      if (_currentMovement == null)
      {
        return;
      }
      string str = e.EditingElement.ToString();
      if (string.IsNullOrWhiteSpace(str))
      {
        return;
      }

      str = str.Replace("System.Windows.Controls.TextBox: ", string.Empty);
      int column = e.Column.DisplayIndex;

      if (column == MOVEMENT_COL_INDEX)
      {
        _currentMovement.Name = str;
      }
      else if (column == NOTES_COL_INDEX)
      {
        _currentMovement.Notes = str;
      }

      _data.UpdateMovement(_currentMovement);
      Debug.WriteLine("movementDataGrid_CellEditEnding UpdateMovement");
    }

    private void movementDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!_isLoading)
      {
        // don't update if editing because for some reason old values still there.
        if (_currentMovement != null && !_isEditing)
        {
          Debug.WriteLine("movementDataGrid_SelectionChanged UpdateMovement");
          // update previous selection
          _data.UpdateMovement(_currentMovement);
        }

        _isEditing = false;
        this.BodyPartMovementsDataGrid.ItemsSource = new List<BodyPartMovement>();
        this.MovementEquipmentsDataGrid.ItemsSource = new List<MovementEquipment>();
        // test SelectedValue because upon hitting enter or tab within the cell will select row beneath
        if (this.movementDataGrid.SelectedValue == null)
        {
          _currentMovement = null;
        }
        else
        {
          _currentMovement = _data.GetMovement((string)this.movementDataGrid.SelectedValue); //(MovementDoc)this.movementDataGrid.SelectedItem; // 
          this.BodyPartMovementsDataGrid.ItemsSource = _currentMovement.BodyParts;
          this.MovementEquipmentsDataGrid.ItemsSource = _currentMovement.Equipment;
        }
      }
    }

    private void NewMovementButton_Click(object sender, RoutedEventArgs e)
    {
      _currentMovement = _data.CreateNewMovement();
      //this.Refresh();
      //int idx = this.movementDataGrid.Items.IndexOf(_currentMovement);
      //this.movementDataGrid.SelectedIndex = idx;

      //_currentMovement = (MovementDoc)this.movementDataGrid.Items.GetItemAt(idx);
      this.movementDataGrid.ItemsSource = _data.GetMovements();
      this.movementDataGrid.SelectedItem = _currentMovement;
    }

    private void AddBodyPartToMovementButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.movementDataGrid.SelectedItems.Count == 0)
      {
        MessageBox.Show("No movement selected");
        return;
      }

      if (this.BodyPartComboBox.SelectedIndex == -1)
      {
        MessageBox.Show("No body part selected");
        return;
      }

      string msg = string.Format("Add body part {0} to all the {1} selected Movements?",
        this.BodyPartComboBox.Text,
        this.movementDataGrid.SelectedItems.Count);
      MessageBoxResult result = MessageBox.Show(msg, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
      if (result == MessageBoxResult.No)
      {
        return;
      }

      for (int i = 0; i < this.movementDataGrid.SelectedItems.Count; ++i)
      {
        MovementDoc movement = (MovementDoc)this.movementDataGrid.SelectedItems[i];
        _currentMovement = _data.AddBodyPartToMovement((string)this.BodyPartComboBox.SelectedValue, movement.Id);
      }

      this.BodyPartMovementsDataGrid.ItemsSource = _currentMovement.BodyParts;
    }

    private void AddBodyPartButton_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.BodyPartToAdd.Text))
      {
        MessageBox.Show("Invalid Body Part");
        return;
      }

      _data.CreateNewBodyPart(this.BodyPartToAdd.Text);
      this.BodyPartToAdd.Text = string.Empty;
      this.BodyPartComboBox.ItemsSource = _data.GetBodyParts();
    }

    private void FindMovementsByBodyPart_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.BodyPartComboBox.Text))
      {
        MessageBox.Show("Invalid Body Part");
        return;
      }

      _isLoading = true;
      string bodyid = (string)this.BodyPartComboBox.SelectedValue;

      this.movementDataGrid.ItemsSource = _data.GetMovementsByBodyPart(bodyid);
      _isLoading = false;

      this.movementDataGrid.SelectedIndex = 0;
    }

    private void GetRandomMovementByBodyPart_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.BodyPartComboBox.Text))
      {
        MessageBox.Show("Invalid Body Part");
        return;
      }

      _isLoading = true;
      string bodyid = (string)this.BodyPartComboBox.SelectedValue;

      this.movementDataGrid.ItemsSource = _data.RandomMovementByBodyPart(bodyid);
      _isLoading = false;

      this.movementDataGrid.SelectedIndex = 0;
    }

    private void AddEquipButton_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.EquipToAdd.Text))
      {
        MessageBox.Show("Invalid Equipment");
        return;
      }

      _data.CreateNewEquipment(this.EquipToAdd.Text);
      this.EquipToAdd.Text = string.Empty;
      this.EquipmentComboBox.ItemsSource = _data.GetEquipment();
    }

    private void AddEquipmentToMovement_Click(object sender, RoutedEventArgs e)
    {
      if (this.movementDataGrid.SelectedItems.Count == 0)
      {
        MessageBox.Show("No movement selected");
        return;
      }

      if (this.EquipmentComboBox.SelectedIndex == -1)
      {
        MessageBox.Show("No equipment selected");
        return;
      }

      string msg = string.Format("Add equipment {0} to all the {1} selected Movements?",
        this.EquipmentComboBox.Text,
        this.movementDataGrid.SelectedItems.Count);
      MessageBoxResult result = MessageBox.Show(msg, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
      if (result == MessageBoxResult.No)
      {
        return;
      }

      for (int i = 0; i < movementDataGrid.SelectedItems.Count; ++i)
      {
        MovementDoc movement = (MovementDoc)this.movementDataGrid.SelectedItems[i];
        _currentMovement = _data.AddEquipmentToMovement((string)this.EquipmentComboBox.SelectedValue, movement.Id);
      }

      this.MovementEquipmentsDataGrid.ItemsSource = _currentMovement.Equipment;
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
      this.Refresh();
    }

    private void Refresh()
    {
      _isLoading = true;
      this.movementDataGrid.ItemsSource = _data.GetMovements();
      //_movementViewSource = ((CollectionViewSource)(this.FindResource("movementViewSource")));
      this.BodyPartComboBox.ItemsSource = _data.GetBodyParts();
      this.EquipmentComboBox.ItemsSource = _data.GetEquipment();
      _isLoading = false;
      this.movementDataGrid.SelectedIndex = 0;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
      var _currentMovement = (MovementDoc)this.movementDataGrid.SelectedItem;
      if (_currentMovement != null)
      {
        _data.UpdateMovement(_currentMovement);
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      MovementDoc _currentMovement = this.movementDataGrid.SelectedItem as MovementDoc;
      if (_currentMovement != null)
      {
        _data.UpdateMovement(_currentMovement);
      }
      //if (_newMovement && _currentMovement != null)
      //{
      //  e.Cancel = PromptSaveChangesCancelled();
      //}
    }

    private bool PromptSaveChangesCancelled()
    {
      string msg = "There are unsaved changes. Do you want to save them?";
      var result = MessageBox.Show(msg, "Wosu", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
      bool cancelOperation = false;
      switch (result)
      {
        case MessageBoxResult.Yes:
          if (_currentMovement != null)
          {
            _data.UpdateMovement(_currentMovement);
          }
          break;
        case MessageBoxResult.No:
          break;
        case MessageBoxResult.Cancel:
          cancelOperation = true;
          break;
      }

      return cancelOperation;
    }

    private void BackupDbButton_Click(object sender, RoutedEventArgs e)
    {
      string path = ConfigurationManager.AppSettings["BackupPath"].ToString();
      if (string.IsNullOrWhiteSpace(path))
      {
        MessageBox.Show("Unspecified BackupPath in App.config");
        return;
      }

      string backupLocation = _data.BackupDb(path);
      string zipLocation = this.ZipBackupFile(path);
      if (!string.IsNullOrWhiteSpace(zipLocation) && File.Exists(zipLocation))
      {
        backupLocation = zipLocation;
      }

      if (File.Exists(backupLocation) &&
        MessageBoxResult.Yes == MessageBox.Show("Open backup location?", "Select File", MessageBoxButton.YesNo, MessageBoxImage.Question))
      {
        string argument = $"/select,\"{backupLocation}\"";
        Process.Start("explorer.exe", argument);
      }
    }

    private string ZipBackupFile(string path)
    {
      //// 7z a secure.7z * -pSECRET
      //// Where: 7z: name and path of 7 - Zip executable
      ////        a : add to archive
      ////        secure.7z: name of destination archive
      ////        *: add all files from current directory to destination archive
      ////        - pSECRET: specify the password "SECRET"
      //// Note: If the password contains spaces or special characters, 
      ////       then enclose it in a pair of quotes
      ////       7z a secure.7z * -p"pa$$word @'|"

      string sevenZipExe = @"C:\Program Files\7-Zip\7z.exe";
      if (File.Exists(sevenZipExe) == false)
      {
        MessageBox.Show("7-Zip Not installed", "Backup Not Compressed", MessageBoxButton.OK, MessageBoxImage.Warning);
        return string.Empty;
      }

      string filepath = Path.Combine(path, "Wosu");

      ProcessStartInfo psi = new ProcessStartInfo();
      psi.FileName = "\"C:\\Program Files\\7-Zip\\7z.exe\"";
      psi.Arguments = $"a \"{filepath}{DateTime.Today.ToString("yyyyMMdd")}.7z\" \"{filepath}.bak\"";
      //psi.WindowStyle = ProcessWindowStyle.Hidden;
      Process p = Process.Start(psi);
      p.WaitForExit();
      return $"{filepath}.7z";
    }

    private void movementDataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
    {
      _isEditing = true;
      Debug.WriteLine("movementDataGrid_PreparingCellForEdit");
    }
  }
}
