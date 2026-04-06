using AgenteINV.DataAcces;
using AgenteINV.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AgenteINV.ViewModels;

public class ProveedoresViewModel : BaseViewModel
{
    private readonly IDbContextFactory<InventarioContext> _contextFactory;
    private Proveedor _currentProveedor = new();
    private Proveedor? _selectedProveedor;

    public ProveedoresViewModel(IDbContextFactory<InventarioContext> contextFactory)
    {
        _contextFactory = contextFactory;
        Title = "Proveedores";
        Proveedores = [];
        SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
        DeleteCommand = new Command(async () => await DeleteAsync(), () => !IsBusy && SelectedProveedor is not null);
        NewCommand = new Command(ResetForm);
    }

    public ObservableCollection<Proveedor> Proveedores { get; }

    public Proveedor CurrentProveedor
    {
        get => _currentProveedor;
        set => SetProperty(ref _currentProveedor, value);
    }

    public Proveedor? SelectedProveedor
    {
        get => _selectedProveedor;
        set
        {
            if (SetProperty(ref _selectedProveedor, value) && value is not null)
            {
                CurrentProveedor = new Proveedor
                {
                    Id = value.Id,
                    Nombre = value.Nombre,
                    Contacto = value.Contacto,
                    Direccion = value.Direccion
                };
                ((Command)DeleteCommand).ChangeCanExecute();
            }
        }
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
            var proveedores = await context.Proveedores.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync();
            Proveedores.Clear();
            foreach (var proveedor in proveedores)
            {
                Proveedores.Add(proveedor);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(CurrentProveedor.Nombre))
        {
            return;
        }

        try
        {
            IsBusy = true;
            await using var context = await _contextFactory.CreateDbContextAsync();
            if (CurrentProveedor.Id > 0)
            {
                var entity = await context.Proveedores.FirstAsync(p => p.Id == CurrentProveedor.Id);
                entity.Nombre = CurrentProveedor.Nombre.Trim();
                entity.Contacto = CurrentProveedor.Contacto?.Trim() ?? string.Empty;
                entity.Direccion = CurrentProveedor.Direccion?.Trim() ?? string.Empty;
            }
            else
            {
                await context.Proveedores.AddAsync(new Proveedor
                {
                    Nombre = CurrentProveedor.Nombre.Trim(),
                    Contacto = CurrentProveedor.Contacto?.Trim() ?? string.Empty,
                    Direccion = CurrentProveedor.Direccion?.Trim() ?? string.Empty
                });
            }

            await context.SaveChangesAsync();
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
        if (SelectedProveedor is null || IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entity = await context.Proveedores
                .Include(p => p.Productos)
                .FirstOrDefaultAsync(p => p.Id == SelectedProveedor.Id);
            if (entity is not null)
            {
                context.Productos.RemoveRange(entity.Productos);
                context.Proveedores.Remove(entity);
                await context.SaveChangesAsync();
            }

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
        SelectedProveedor = null;
        CurrentProveedor = new Proveedor();
        ((Command)DeleteCommand).ChangeCanExecute();
    }
}
