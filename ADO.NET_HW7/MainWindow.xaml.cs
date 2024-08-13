using ADO.NET_HW7.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADO.NET_HW7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Books book = new Books();

        public MainWindow()
        {
            InitializeComponent();
            ShowBooks();
            deleteBtn.IsEnabled = false;
            updateBtn.IsEnabled = false;
        }

        private void Clear()
        {
            titleTxtBox.Clear();
            authorTxtBox.Clear();
            categoryTxtBox.Clear();
            publisherTxtBox.Clear();
            yearTxtBox.Clear();
            pagesTxtBox.Clear();
            book.Id = 0;
            deleteBtn.IsEnabled = false;
            updateBtn.IsEnabled = false;
        }

        private void ShowBooks()
        {
            using (LibraryDBEntities db = new LibraryDBEntities())
            {
                dataGridView.ItemsSource = db.Books.ToList();
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titleTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(authorTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(categoryTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(publisherTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(yearTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(pagesTxtBox.Text))
                {
                    MessageBox.Show("Будь ласка, заповніть всі обов'язкові поля перед додаванням книги.", "Ой", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate that year and pages are valid integers
                else if (!int.TryParse(yearTxtBox.Text, out int year) || !int.TryParse(pagesTxtBox.Text, out int pages))
                {
                    MessageBox.Show("Рік видання та кількість сторінок повинні бути цілими числами.", "Ой", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                book.Title = titleTxtBox.Text;
                book.Author = authorTxtBox.Text;
                book.Category = categoryTxtBox.Text;
                book.Publisher = publisherTxtBox.Text;
                book.Year = int.Parse(yearTxtBox.Text);
                book.Pages = int.Parse(pagesTxtBox.Text);

                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    db.Books.Add(book);
                    db.SaveChanges();
                }

                ShowBooks();
                MessageBox.Show("Дані додано успішно");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
            finally
            {
                Clear();
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                book.Title = titleTxtBox.Text;
                book.Author = authorTxtBox.Text;
                book.Category = categoryTxtBox.Text;
                book.Publisher = publisherTxtBox.Text;
                book.Year = int.Parse(yearTxtBox.Text);
                book.Pages = int.Parse(pagesTxtBox.Text);

                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    if (book.Id != 0)
                    {
                        db.Entry(book).State = EntityState.Modified;
                    }
                    else
                    {
                        MessageBox.Show("Ключ запису недійсний");
                    }
                    db.SaveChanges();
                }

                Clear();
                ShowBooks();
                MessageBox.Show("Дані змінено успішно");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
            finally
            {
                Clear();
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Ви дійсно хочете видалити цю книгу?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (LibraryDBEntities db = new LibraryDBEntities())
                    {
                        DbEntityEntry<Books> entry = db.Entry(book);
                        if (entry.State == EntityState.Detached)
                        {
                            db.Books.Attach(book);
                        }

                        db.Books.Remove(book);
                        db.SaveChanges();

                        Clear();
                        ShowBooks();
                        MessageBox.Show("Дані видалено успішно");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
            finally
            {
                Clear();
            }
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string title = titleTxtBox.Text;
                string author = authorTxtBox.Text;
                string category = categoryTxtBox.Text;
                string publisher = publisherTxtBox.Text;
                
                int? year = !string.IsNullOrWhiteSpace(yearTxtBox.Text) 
                    ? int.Parse(yearTxtBox.Text) : (int?)null;

                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    IQueryable<Books> query = db.Books.AsQueryable();
                    
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        query = query.Where(b => b.Title.ToLower()
                        .Contains(title.ToLower()));
                    }
                    if (!string.IsNullOrWhiteSpace(author))
                    {
                        query = query.Where(b => b.Author.ToLower()
                        .Contains(author.ToLower()));
                    }
                    if (!string.IsNullOrWhiteSpace(category))
                    {
                        query = query.Where(b => b.Category.ToLower()
                        .Contains(category.ToLower()));
                    }
                    if (!string.IsNullOrWhiteSpace(publisher))
                    {
                        query = query.Where(b => b.Publisher.ToLower()
                        .Contains(publisher.ToLower()));
                    }
                    if (year.HasValue)
                    {
                        query = query.Where(b => b.Year == year.Value);
                    }
                    
                    List<Books> searchResults = query.ToList();
                    dataGridView.ItemsSource = searchResults;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private void dataGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //var currentRowIndex = dataGridView.Items.IndexOf(dataGridView.CurrentItem);
            try
            {
                if (dataGridView.CurrentItem is Books selectedBook)
                {
                    titleTxtBox.Text = selectedBook.Title;
                    authorTxtBox.Text = selectedBook.Author;
                    categoryTxtBox.Text = selectedBook.Category;
                    publisherTxtBox.Text = selectedBook.Publisher;
                    yearTxtBox.Text = selectedBook.Year.ToString();
                    pagesTxtBox.Text = selectedBook.Pages.ToString();

                    book.Id = selectedBook.Id;

                    deleteBtn.IsEnabled = true;
                    updateBtn.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }
    }
}