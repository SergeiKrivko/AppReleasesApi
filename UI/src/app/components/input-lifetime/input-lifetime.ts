import {Component, DestroyRef, forwardRef, inject, OnInit} from '@angular/core';
import {TuiInputNumberDirective, TuiSwitch} from '@taiga-ui/kit';
import {ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR, ReactiveFormsModule} from '@angular/forms';
import {duration, Duration} from 'moment';
import {TuiTextfieldComponent} from '@taiga-ui/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {tap} from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-input-lifetime',
  imports: [
    TuiSwitch,
    ReactiveFormsModule,
    TuiInputNumberDirective,
    TuiTextfieldComponent
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputLifetime),
      multi: true
    }
  ],
  templateUrl: './input-lifetime.html',
  styleUrl: './input-lifetime.scss'
})
export class InputLifetime implements ControlValueAccessor, OnInit {
  private readonly destroyRef = inject(DestroyRef);

  protected readonly control = new FormGroup({
    switch: new FormControl<boolean>(false),
    days: new FormControl<number>(0),
  })

  ngOnInit() {
    this.control.valueChanges.pipe(
      tap(() => this.onChange(this.readValue())),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe();
  }

  private onChange: (value: Duration | null) => void = () => {
  };
  private onTouched: () => void = () => {
  };

  private readValue(): Duration | null {
    if (!this.control.value.switch)
      return null;
    return duration(this.control.value.days ?? 0, 'days');
  }

  writeValue(value: Duration | null): void {
    this.control.setValue({
      switch: value !== null,
      days: value?.asDays() ?? 0,
    })
  }

  registerOnChange(fn: (value: Duration | null) => void): void {
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
}
