import { Component, input } from '@angular/core';

@Component({
  selector: 'app-form-label',
  template: `<label [attr.for]="for() ?? null" style="font-size:0.875rem; font-weight:500; color:var(--p-surface-300)">{{ label() }}</label>`,
})
export class FormLabelComponent {
  label = input.required<string>();
  for  = input<string>();
}
