import {ChangeDetectionStrategy, Component, DestroyRef, forwardRef, inject, OnInit} from '@angular/core';
import {ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR, ReactiveFormsModule} from '@angular/forms';
import {tap} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {AsyncPipe} from '@angular/common';
import {TuiChevron, TuiDataListWrapperComponent, TuiSelectDirective} from '@taiga-ui/kit';
import {TuiLabel, TuiTextfield, TuiTextfieldComponent} from '@taiga-ui/core';

@Component({
  standalone: true,
  selector: 'app-console-installer-settings',
  imports: [
    ReactiveFormsModule,
    AsyncPipe,
    TuiChevron,
    TuiDataListWrapperComponent,
    TuiLabel,
    TuiSelectDirective,
    TuiTextfieldComponent,
    TuiTextfield
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ConsoleInstallerSettings),
      multi: true
    }
  ],
  templateUrl: './console-installer-settings.component.html',
  styleUrl: './console-installer-settings.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConsoleInstallerSettings implements ControlValueAccessor, OnInit {
  private readonly destroyRef = inject(DestroyRef);

  protected readonly control = new FormGroup({
    installerPlatform: new FormControl<string | null>(null),
  })

  ngOnInit() {
    this.control.valueChanges.pipe(
      tap(() => this.onChange(this.control.value)),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe();
  }

  private onChange: (value: any) => void = () => {
  };
  private onTouched: () => void = () => {
  };

  writeValue(value: any): void {
    this.control.setValue({
      installerPlatform: value?.installerPlatform ?? this.installerPlatforms$[0],
    })
  }

  registerOnChange(fn: (value: any) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    if (isDisabled)
      this.control.disable();
    else
      this.control.enable();
  }

  protected readonly installerPlatforms$: string[] = [
    "win-x64",
    "win-arm64",
    "linux-x64",
    "linux-arm64",
    "osx-x64",
    "osx-arm64",
  ]
}
