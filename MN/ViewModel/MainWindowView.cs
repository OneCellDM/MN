using MN.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MN.ViewModel
{
    class MainWindowView: INotifyPropertyChanged
    {
        private NoteModel _currentNoteModel;
        public NoteModel CurrentNoteModel
        {
            get => _currentNoteModel;
            set
            {
                _currentNoteModel = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<NoteModel> NoteModelCollection { get; set; }

        public MainWindowView()
        {
            NoteModelCollection = new ObservableCollection<NoteModel>();
            LoadCollections();
        }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand { get {

                return _saveCommand ?? (_saveCommand = new RelayCommand(obj =>
                    {
                        SaveItem();
                    }
                ));
            } 
        }
        private RelayCommand _add;
        public RelayCommand Add
        {
            get
            {

                return _add ?? (_add = new RelayCommand(obj =>
                {

                    if (CurrentNoteModel != null)
                        if (CurrentNoteModel.Title.Length != 0 & CurrentNoteModel.Data.Length != 0)
                        {
                            SaveItem();
                            AddItem();
                        }
                        else return;
                    else
                        AddItem();
                }) );
            }
        }

        private RelayCommand _remove;
        public RelayCommand Remove
        {
            get
            {

                return _remove ?? (_remove = new RelayCommand(obj =>
                {
                    
                    RemoveItem();
                    NoteModelCollection.Remove(CurrentNoteModel);
                    CurrentNoteModel = null;
                    
                },(obj)=>CurrentNoteModel!=null));
            }
        }
        private void AddItem()
        {

            var model = new NoteModel() { Id = GetMaxId(), IsNew = true };
            
            NoteModelCollection.Insert(0, model);
            CurrentNoteModel = model;
        }
        private int GetMaxId()
        {
            int Max = 1;
            foreach(var k in NoteModelCollection)
                if (Max < k.Id)
                    Max = k.Id;

            return Max+1;
        }
        
        public void RemoveItem()
        {
            
            XmlDocument xDoc = new XmlDocument();
            
            xDoc.Load("Notes.xml");
            var ie = xDoc.DocumentElement.GetEnumerator();
            while (ie.MoveNext())   // пока не будет возвращено false
            {
                var ID = int.Parse(((XmlElement)ie.Current).Attributes.GetNamedItem("ID").Value);
                if(CurrentNoteModel.Id==ID)
                {
                    xDoc.DocumentElement.RemoveChild((XmlElement)ie.Current);
                    xDoc.Save("Notes.xml");
                    break;
                }
            }

            }

        public void SaveItem()
        {
            
            if (!File.Exists("Notes.xml"))
            {
                new XDocument(new XElement("Notes",
                    new XElement("Note",
                        new XAttribute("Title", CurrentNoteModel.Title),
                        new XAttribute("ID", CurrentNoteModel.Id),
                        new XElement("Data", CurrentNoteModel.Data)
                        )
                    )).Save("Notes.xml");
            }
            else
            {
                if (CurrentNoteModel.IsNew == true)
                {

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load("Notes.xml");
                    XmlElement xRoot = xDoc.DocumentElement;
                   
                    XmlElement Note = xDoc.CreateElement("Note");
                   
                    XmlAttribute Title = xDoc.CreateAttribute("Title");
                    XmlAttribute ID = xDoc.CreateAttribute("ID");

                    XmlElement Data = xDoc.CreateElement("Data");
                   
                    XmlText TitleValue = xDoc.CreateTextNode(CurrentNoteModel.Title);
                    XmlText IDValue = xDoc.CreateTextNode(CurrentNoteModel.Id.ToString());
                    XmlText DataValue = xDoc.CreateTextNode(CurrentNoteModel.Data);

                    Title.AppendChild(TitleValue);
                    ID.AppendChild(IDValue);
                    Data.AppendChild(DataValue);
                   
                    Note.Attributes.Append(Title);
                    Note.Attributes.Append(ID);
                    Note.AppendChild(Data);
                 
                    xRoot.AppendChild(Note);

                    CurrentNoteModel.IsNew = false;
                    xDoc.Save("Notes.xml");
                }
                else
                {

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load("Notes.xml");
                    XmlElement xRoot = xDoc.DocumentElement;
                    foreach (XmlElement xnode in xRoot)
                    {
                        
                        
                            var attr = xnode.Attributes.GetNamedItem("ID");
                        if (attr != null)
                            if(int.Parse(attr.Value)==CurrentNoteModel.Id)
                            {
                                Debug.WriteLine("ofdf");
                                XmlNode attr2 = xnode.Attributes.GetNamedItem("Title");
                                if (attr2 != null)
                                    attr2.Value = CurrentNoteModel.Title;

                                foreach (XmlNode childnode in xnode.ChildNodes)
                                {
                                    if (childnode.Name == "Data")
                                        childnode.InnerText = CurrentNoteModel.Data;

                                }
                            }
                    }
                    xDoc.Save("Notes.xml");
                    
                }
            }
   
            
        }
       
       public void LoadCollections()
       {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("Notes.xml");
                XmlElement xRoot = xDoc.DocumentElement;
                foreach (XmlElement xnode in xRoot)
                {
                    NoteModel Note = new NoteModel();
                    XmlNode attr = xnode.Attributes.GetNamedItem("Title");
                    if (attr != null)
                        Note.Title = attr.Value;
                    attr = xnode.Attributes.GetNamedItem("ID");
                    if (attr != null)
                        Note.Id = int.Parse(attr.Value);

                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        if (childnode.Name == "Data")
                            Note.Data = childnode.InnerText;
                    }
                    Note.IsNew = false;
                    NoteModelCollection.Insert(0, Note);
                }
            }
            catch(Exception ex) { Debug.WriteLine(ex); }
       }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
