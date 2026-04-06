using AgenteINV.DataAcces;
using AgenteINV.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AgenteINV.ViewModels;

public class CategoriasViewModel : BaseViewModel
{
    private readonly IDbContextFactory<InventarioContext> _contextFactory;
    private Categoria _currentCategoria = new();
    private Categoria? _selectedCategoria;
    private string _statusMessage = string.Empty;

    public CategoriasViewModel(IDbContextFactory<InventarioContext> contextFactory)
    {
        _contextFactory = contextFactory;
        Title = "Categorías";
        Categorias = [];
        SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
        DeleteCommand = new Command(async () => await DeleteAsync(), () => !IsBusy && SelectedCategoria is not null);
        NewCommand = new Command(ResetForm);
    }

    public ObservableCollection<Categoria> Categorias { get; }

    public Categoria CurrentCategoria
    {
        get => _currentCategoria;
        set => SetProperty(ref _currentCategoria, value);
    }

    public Categoria? SelectedCategoria
    {
        get => _selectedCategoria;
        set
        {
            if (SetProperty(ref _selectedCategoria, value) && value is not null)
            {
                CurrentCategoria = new Categoria { Id = value.Id, Nombre = value.Nombre };
                ((Command)DeleteCommand).ChangeCanExecute();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand NewCommand { get; }

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await using var context = await _contextFactory.CreateDbContextAsync();
            var categorias = await context.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync();
            Categorias.Clear();
            foreach (var categoria in categorias)
            {
                Categorias.Add(categoria);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(CurrentCategoria.Nombre))
        {
            return;
        }

        try
        {
            IsBusy = true;
            await using var context = await _contextFactory.CreateDbContextAsync();
            if (CurrentCategoria.Id > 0)
            {
                var entity = await context.Categorias.FirstAsync(c => c.Id == CurrentCategoria.Id);
                entity.Nombre = CurrentCategoria.Nombre.Trim();
            }
            else
            {
                await context.Categorias.AddAsync(new Categoria { Nombre = CurrentCategoria.Nombre.Trim() });
            }

            await context.SaveChangesAsync();
            StatusMessage = string.Empty;
            ResetForm();
            await LoadAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedCategoria is null || IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entity = await context.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == SelectedCategoria.Id);
            if (entity is not null)
            {
                if (entity.Productos.Count > 0)
                {
                    StatusMessage = "No se puede eliminar una categoría con productos asociados.";
                    return;
                }

                context.Categorias.Remove(entity);
                await context.SaveChangesAsync();
            }

            StatusMessage = string.Empty;
            ResetForm();
            await LoadAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ResetForm()
    {
        SelectedCategoria = null;
        CurrentCategoria = new Categoria();
        ((Command)DeleteCommand).ChangeCanExecute();
    }
}
