import { Component, ElementRef, OnInit, ViewChild, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { finalize, forkJoin, switchMap } from 'rxjs';
import { PatientService } from '../../../core/services/patient.service';
import { MasterDataService, MasterDataItem } from '../../../core/services/master-data.service';
import { LoadingService } from '../../../core/services/loading.service';
import { NotificationService } from '../../../core/services/notification.service';
import { CreatePatientRequest, PatientDetail, PatientSummary } from '../../../core/models/patient.model';
import { PatientSearchDialogComponent } from './patient-search-dialog.component';

export type PatientRegistrationType = 'new' | 'existing' | 'staff' | 'newborn';

interface StoredDocument {
  id: string;
  name: string;
  size: number;
  mimeType: string;
  uploadedAt: string;
  dataUrl: string;
}

interface RegistrationMeta {
  patientType?: PatientRegistrationType;
  appointmentReference?: string;
  prefix?: string;
  maritalStatus?: string;
  race?: string;
  religion?: string;
  language?: string;
  medicalWarnings?: string;
  area?: string;
  professional?: {
    occupation?: string;
    company?: string;
    profession?: string;
    incomeCategory?: string;
    officeAddress?: string;
  };
  staff?: { employeeId?: string; department?: string };
  newborn?: { motherMrn?: string; birthWeight?: string; birthTime?: string };
  documentsPlaceholder?: string;
  documents?: StoredDocument[];
}

@Component({
  selector: 'app-patient-registration',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatExpansionModule,
    MatDialogModule,
    MatChipsModule,
    MatDividerModule,
  ],
  templateUrl: './patient-registration.component.html',
  styleUrl: './patient-registration.component.scss',
})
export class PatientRegistrationComponent implements OnInit {
  @ViewChild('documentInput') documentInput?: ElementRef<HTMLInputElement>;

  private readonly fb = inject(FormBuilder);
  private readonly patientService = inject(PatientService);
  private readonly masterData = inject(MasterDataService);
  private readonly router = inject(Router);
  private readonly loading = inject(LoadingService);
  private readonly notifications = inject(NotificationService);
  private readonly dialog = inject(MatDialog);

  readonly patientTypes: { value: PatientRegistrationType; label: string; icon: string }[] = [
    { value: 'new', label: 'New Registration', icon: 'person_add' },
    { value: 'existing', label: 'Existing Search & Update', icon: 'manage_search' },
    { value: 'staff', label: 'Staff Patient', icon: 'badge' },
    { value: 'newborn', label: 'Newborn Patient', icon: 'child_care' },
  ];

  readonly editPatientId = signal<string | null>(null);
  readonly selectedPatientLabel = signal<string | null>(null);
  readonly documents = signal<StoredDocument[]>([]);
  readonly recentPatients = signal<PatientSummary[]>([]);

  readonly maxDocumentBytes = 5 * 1024 * 1024;
  readonly acceptedDocumentTypes = ['application/pdf', 'image/jpeg', 'image/jpg', 'image/png'];

  readonly prefixes = signal<MasterDataItem[]>([]);
  readonly genders = signal<MasterDataItem[]>([]);
  readonly bloodGroups = signal<MasterDataItem[]>([]);
  readonly maritalStatuses = signal<MasterDataItem[]>([]);
  readonly nationalities = signal<MasterDataItem[]>([]);
  readonly races = signal<MasterDataItem[]>([]);
  readonly religions = signal<MasterDataItem[]>([]);
  readonly languages = signal<MasterDataItem[]>([]);
  readonly countries = signal<MasterDataItem[]>([]);
  readonly states = signal<MasterDataItem[]>([]);
  readonly cities = signal<MasterDataItem[]>([]);
  readonly areas = signal<MasterDataItem[]>([]);
  readonly occupations = signal<MasterDataItem[]>([]);
  readonly companies = signal<MasterDataItem[]>([]);
  readonly professions = signal<MasterDataItem[]>([]);
  readonly incomeCategories = signal<MasterDataItem[]>([]);
  readonly relationships = signal<MasterDataItem[]>([]);

  readonly form = this.fb.nonNullable.group({
    patientType: ['new' as PatientRegistrationType],
    appointmentReference: [''],

    prefix: [''],
    firstName: ['', Validators.required],
    middleName: [''],
    lastName: ['', Validators.required],
    dateOfBirth: ['', Validators.required],
    ageYears: [{ value: 0, disabled: true }],
    ageMonths: [{ value: 0, disabled: true }],
    ageDays: [{ value: 0, disabled: true }],
    gender: ['', Validators.required],
    bloodGroup: [''],
    mobileCountryCode: ['+91'],
    mobile: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    email: ['', Validators.email],
    nationalId: [''],
    maritalStatus: [''],

    nationality: [''],
    race: [''],
    religion: [''],
    language: [''],
    medicalWarnings: [''],

    addressType: ['Home'],
    addressLine1: [''],
    addressLine2: [''],
    country: ['IN'],
    state: [''],
    city: [''],
    area: [''],
    postalCode: ['', Validators.pattern(/^(\d{6})?$/)],

    occupation: [''],
    company: [''],
    profession: [''],
    incomeCategory: [''],
    officeAddress: [''],

    employeeId: [''],
    staffDepartment: [''],

    motherMrn: [''],
    birthWeight: [''],
    birthTime: [''],

    insuranceProvider: [''],
    insurancePolicyNumber: [''],
    insuranceGroupNumber: [''],
    insuranceExpiry: [''],
    sponsorName: [''],

    emergencyName: [''],
    emergencyRelationship: [''],
    emergencyPhone: [''],
    emergencyEmail: [''],

    documentNotes: [''],
    notes: [''],
  });

  ngOnInit(): void {
    this.loadStaticDropdowns();
    this.loadRecentPatients();
    this.loadStates('IN');
    this.bindAgeCalculation();
    this.bindAddressCascade();
    this.form.controls.patientType.valueChanges.subscribe((type) => {
      if (type === 'existing' && !this.editPatientId()) {
        this.openSearchDialog();
      }
    });
  }

  get isEditMode(): boolean {
    return !!this.editPatientId();
  }

  get isStaffType(): boolean {
    return this.form.controls.patientType.value === 'staff';
  }

  get isNewbornType(): boolean {
    return this.form.controls.patientType.value === 'newborn';
  }

  get isExistingType(): boolean {
    return this.form.controls.patientType.value === 'existing';
  }

  openSearchDialog(): void {
    const ref = this.dialog.open(PatientSearchDialogComponent, { width: '560px' });
    ref.afterClosed().subscribe((summary) => {
      if (summary?.id) {
        this.loadPatientForEdit(summary.id, summary.fullName);
      }
    });
  }

  newForm(): void {
    this.editPatientId.set(null);
    this.selectedPatientLabel.set(null);
    this.documents.set([]);
    this.form.reset({
      patientType: 'new',
      mobileCountryCode: '+91',
      country: 'IN',
      addressType: 'Home',
      ageYears: 0,
      ageMonths: 0,
      ageDays: 0,
    });
    this.loadStates('IN');
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.error('Please correct the highlighted fields');
      return;
    }

    const request = this.buildRequest();
    const editId = this.editPatientId();

    if (!editId) {
      this.loading.show();
      this.patientService
        .checkDuplicates({
          firstName: request.firstName,
          lastName: request.lastName,
          dateOfBirth: request.dateOfBirth,
          nationalId: request.nationalId,
          phone: request.contacts.find((c) => c.isPrimary)?.value,
        })
        .pipe(
          switchMap((dup) => {
            if (dup.data?.hasDuplicates) {
              this.notifications.info('Potential duplicate found — saving with override for POC.');
              request.allowDuplicateOverride = true;
            }
            return this.patientService.create(request);
          }),
          finalize(() => this.loading.hide()),
        )
        .subscribe({
          next: (response) => {
            if (response.success) {
              this.notifications.success('Patient registered successfully!');
              void this.router.navigate(['/patients']);
            }
          },
        });
      return;
    }

    this.loading.show();
    this.patientService
      .update(editId, request)
      .pipe(finalize(() => this.loading.hide()))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notifications.success('Patient updated successfully!');
            void this.router.navigate(['/patients', editId]);
          }
        },
      });
  }

  reset(): void {
    if (this.isEditMode) {
      const id = this.editPatientId();
      const label = this.selectedPatientLabel();
      if (id && label) {
        this.loadPatientForEdit(id, label);
        return;
      }
    }
    this.newForm();
  }

  triggerDocumentUpload(): void {
    this.documentInput?.nativeElement.click();
  }

  onDocumentsSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files;
    if (!files?.length) return;

    Array.from(files).forEach((file) => {
      if (!this.acceptedDocumentTypes.includes(file.type)) {
        this.notifications.error(`${file.name}: only PDF, JPG, and PNG files are allowed`);
        return;
      }
      if (file.size > this.maxDocumentBytes) {
        this.notifications.error(`${file.name}: file must be 5MB or smaller`);
        return;
      }

      const reader = new FileReader();
      reader.onload = () => {
        const dataUrl = reader.result as string;
        this.documents.update((list) => [
          ...list,
          {
            id: crypto.randomUUID(),
            name: file.name,
            size: file.size,
            mimeType: file.type,
            uploadedAt: new Date().toISOString(),
            dataUrl,
          },
        ]);
        this.notifications.success(`Uploaded ${file.name}`);
      };
      reader.onerror = () => this.notifications.error(`Failed to read ${file.name}`);
      reader.readAsDataURL(file);
    });

    input.value = '';
  }

  removeDocument(id: string): void {
    this.documents.update((list) => list.filter((doc) => doc.id !== id));
  }

  previewDocument(doc: StoredDocument): void {
    window.open(doc.dataUrl, '_blank', 'noopener,noreferrer');
  }

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .filter(Boolean)
      .map((part) => part[0])
      .join('')
      .slice(0, 2)
      .toUpperCase();
  }

  statusClass(status: string): string {
    if (status === 'Critical') return 'status-critical';
    if (status === 'Active') return 'status-active';
    return 'status-default';
  }

  private loadRecentPatients(): void {
    this.patientService.search(undefined, 1, 8).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.recentPatients.set(response.data.items);
        }
      },
    });
  }

  private loadStaticDropdowns(): void {
    forkJoin({
      prefixes: this.masterData.getByType('Prefix'),
      genders: this.masterData.getByType('Gender'),
      bloodGroups: this.masterData.getByType('BloodGroup'),
      maritalStatuses: this.masterData.getByType('MaritalStatus'),
      nationalities: this.masterData.getByType('Nationality'),
      races: this.masterData.getByType('Race'),
      religions: this.masterData.getByType('Religion'),
      languages: this.masterData.getByType('Language'),
      countries: this.masterData.getByType('Country'),
      occupations: this.masterData.getByType('Occupation'),
      companies: this.masterData.getByType('Company'),
      professions: this.masterData.getByType('Profession'),
      incomeCategories: this.masterData.getByType('IncomeCategory'),
      relationships: this.masterData.getByType('Relationship'),
    }).subscribe((data) => {
      this.prefixes.set(data.prefixes);
      this.genders.set(data.genders);
      this.bloodGroups.set(data.bloodGroups);
      this.maritalStatuses.set(data.maritalStatuses);
      this.nationalities.set(data.nationalities);
      this.races.set(data.races);
      this.religions.set(data.religions);
      this.languages.set(data.languages);
      this.countries.set(data.countries);
      this.occupations.set(data.occupations);
      this.companies.set(data.companies);
      this.professions.set(data.professions);
      this.incomeCategories.set(data.incomeCategories);
      this.relationships.set(data.relationships);
    });
  }

  private bindAgeCalculation(): void {
    this.form.controls.dateOfBirth.valueChanges.subscribe((dob) => this.updateAgeFields(dob));
  }

  private bindAddressCascade(): void {
    this.form.controls.country.valueChanges.subscribe((code) => {
      this.form.patchValue({ state: '', city: '', area: '' }, { emitEvent: false });
      this.cities.set([]);
      this.areas.set([]);
      if (code) this.loadStates(code);
    });
    this.form.controls.state.valueChanges.subscribe((code) => {
      this.form.patchValue({ city: '', area: '' }, { emitEvent: false });
      this.areas.set([]);
      if (code) this.loadCities(code);
    });
    this.form.controls.city.valueChanges.subscribe((code) => {
      this.form.patchValue({ area: '' }, { emitEvent: false });
      if (code) this.loadAreas(code);
    });
  }

  private loadStates(countryCode: string): void {
    this.masterData.getByType('State', countryCode).subscribe((items) => this.states.set(items));
  }

  private loadCities(stateCode: string): void {
    this.masterData.getByType('City', stateCode).subscribe((items) => this.cities.set(items));
  }

  private loadAreas(cityCode: string): void {
    this.masterData.getByType('Area', cityCode).subscribe((items) => this.areas.set(items));
  }

  private updateAgeFields(dob: string): void {
    if (!dob) {
      this.form.patchValue({ ageYears: 0, ageMonths: 0, ageDays: 0 }, { emitEvent: false });
      return;
    }
    const birth = new Date(dob);
    const today = new Date();
    let years = today.getFullYear() - birth.getFullYear();
    let months = today.getMonth() - birth.getMonth();
    let days = today.getDate() - birth.getDate();
    if (days < 0) {
      months -= 1;
      const prevMonth = new Date(today.getFullYear(), today.getMonth(), 0);
      days += prevMonth.getDate();
    }
    if (months < 0) {
      years -= 1;
      months += 12;
    }
    this.form.patchValue({ ageYears: years, ageMonths: months, ageDays: days }, { emitEvent: false });
  }

  private loadPatientForEdit(id: string, label: string): void {
    this.loading.show();
    this.patientService
      .getById(id)
      .pipe(finalize(() => this.loading.hide()))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.patchFromPatient(response.data);
            this.editPatientId.set(id);
            this.selectedPatientLabel.set(label);
            this.form.patchValue({ patientType: 'existing' });
            this.notifications.info(`Loaded patient: ${label}`);
          }
        },
      });
  }

  private patchFromPatient(patient: PatientDetail): void {
    const meta = this.parseNotesMeta(patient.notes);
    const home = patient.addresses.find((a) => a.addressType === 'Home') ?? patient.addresses[0];
    const phone = patient.contacts.find((c) => c.contactType === 'Mobile' || c.contactType === 'Phone');
    const email = patient.contacts.find((c) => c.contactType === 'Email');

    this.form.patchValue({
      appointmentReference: meta.appointmentReference ?? patient.legacyPatientId ?? '',
      prefix: meta.prefix ?? '',
      firstName: patient.firstName,
      middleName: patient.middleName ?? '',
      lastName: patient.lastName,
      dateOfBirth: patient.dateOfBirth,
      gender: patient.gender,
      bloodGroup: patient.bloodGroup,
      mobile: phone?.value?.replace(/\D/g, '').slice(-10) ?? '',
      email: email?.value ?? '',
      nationalId: patient.nationalId ?? '',
      nationality: patient.nationality ?? '',
      maritalStatus: meta.maritalStatus ?? '',
      race: meta.race ?? '',
      religion: meta.religion ?? '',
      language: meta.language ?? '',
      medicalWarnings: meta.medicalWarnings ?? '',
      addressLine1: home?.line1 ?? '',
      addressLine2: home?.line2 ?? '',
      country: home?.country ?? 'IN',
      state: home?.state ?? '',
      city: home?.city ?? '',
      area: meta.area ?? '',
      postalCode: home?.postalCode ?? '',
      occupation: meta.professional?.occupation ?? '',
      company: meta.professional?.company ?? '',
      profession: meta.professional?.profession ?? '',
      incomeCategory: meta.professional?.incomeCategory ?? '',
      officeAddress: meta.professional?.officeAddress ?? '',
      employeeId: meta.staff?.employeeId ?? '',
      staffDepartment: meta.staff?.department ?? '',
      motherMrn: meta.newborn?.motherMrn ?? '',
      birthWeight: meta.newborn?.birthWeight ?? '',
      birthTime: meta.newborn?.birthTime ?? '',
      documentNotes: meta.documentsPlaceholder ?? '',
      notes: this.extractAdminNotes(patient.notes),
    });

    this.documents.set(meta.documents ?? []);

    if (home?.country) this.loadStates(home.country);
    if (home?.state) this.loadCities(home.state);
    if (home?.city) this.loadAreas(home.city);
    this.updateAgeFields(patient.dateOfBirth);

    const emergency = patient.emergencyContacts[0];
    if (emergency) {
      this.form.patchValue({
        emergencyName: emergency.name,
        emergencyRelationship: emergency.relationship,
        emergencyPhone: emergency.phone,
        emergencyEmail: emergency.email ?? '',
      });
    }

    const insurance = patient.insurances[0] as {
      providerName?: string;
      policyNumber?: string;
      groupNumber?: string;
      expiryDate?: string;
    } | undefined;
    if (insurance) {
      this.form.patchValue({
        insuranceProvider: insurance.providerName ?? '',
        insurancePolicyNumber: insurance.policyNumber ?? '',
        insuranceGroupNumber: insurance.groupNumber ?? '',
        insuranceExpiry: insurance.expiryDate ?? '',
      });
    }
  }

  private buildRequest(): CreatePatientRequest {
    const v = this.form.getRawValue();
    const meta: RegistrationMeta = {
      patientType: v.patientType,
      appointmentReference: v.appointmentReference,
      prefix: v.prefix,
      maritalStatus: v.maritalStatus,
      race: v.race,
      religion: v.religion,
      language: v.language,
      medicalWarnings: v.medicalWarnings,
      area: v.area,
      professional: {
        occupation: v.occupation,
        company: v.company,
        profession: v.profession,
        incomeCategory: v.incomeCategory,
        officeAddress: v.officeAddress,
      },
      staff: v.patientType === 'staff' ? { employeeId: v.employeeId, department: v.staffDepartment } : undefined,
      newborn:
        v.patientType === 'newborn'
          ? { motherMrn: v.motherMrn, birthWeight: v.birthWeight, birthTime: v.birthTime }
          : undefined,
      documentsPlaceholder: v.documentNotes,
      documents: this.documents(),
    };

    const addresses = [];
    if (v.addressLine1) {
      addresses.push({
        addressType: v.addressType || 'Home',
        line1: v.addressLine1,
        line2: v.addressLine2 || null,
        city: v.city || v.area || 'Unknown',
        state: v.state || null,
        postalCode: v.postalCode || null,
        country: v.country || 'IN',
        isPrimary: true,
      });
    }
    if (v.officeAddress) {
      addresses.push({
        addressType: 'Work',
        line1: v.officeAddress,
        city: v.city || 'Unknown',
        state: v.state || null,
        country: v.country || 'IN',
        isPrimary: false,
      });
    }

    const contacts = [
      {
        contactType: 'Mobile',
        value: `${v.mobileCountryCode}${v.mobile}`.replace('++', '+'),
        isPrimary: true,
      },
    ];
    if (v.email) {
      contacts.push({ contactType: 'Email', value: v.email, isPrimary: false });
    }

    const emergencyContacts = v.emergencyPhone
      ? [
          {
            name: v.emergencyName || 'Emergency Contact',
            relationship: v.emergencyRelationship || 'Other',
            phone: v.emergencyPhone,
            email: v.emergencyEmail || null,
            isPrimary: true,
          },
        ]
      : [];

    const insurances = v.insuranceProvider
      ? [
          {
            providerName: v.insuranceProvider,
            policyNumber: v.insurancePolicyNumber || 'PENDING',
            groupNumber: v.insuranceGroupNumber || null,
            expiryDate: v.insuranceExpiry || null,
            isPrimary: true,
          },
        ]
      : [];

    return {
      firstName: v.firstName,
      middleName: v.middleName || null,
      lastName: v.lastName,
      dateOfBirth: v.dateOfBirth,
      gender: v.gender,
      bloodGroup: v.bloodGroup || 'Unknown',
      nationalId: v.nationalId || null,
      nationality: v.nationality || null,
      legacyPatientId: v.appointmentReference || v.employeeId || null,
      notes: this.serializeNotes(v.notes, meta),
      allowDuplicateOverride: false,
      addresses,
      contacts,
      emergencyContacts,
      insurances,
    };
  }

  private serializeNotes(adminNotes: string, meta: RegistrationMeta): string {
    return JSON.stringify({ adminNotes, registrationMeta: meta });
  }

  private parseNotesMeta(notes: string | null): RegistrationMeta {
    if (!notes) return {};
    try {
      const parsed = JSON.parse(notes) as { registrationMeta?: RegistrationMeta };
      return parsed.registrationMeta ?? {};
    } catch {
      return {};
    }
  }

  private extractAdminNotes(notes: string | null): string {
    if (!notes) return '';
    try {
      const parsed = JSON.parse(notes) as { adminNotes?: string };
      return parsed.adminNotes ?? '';
    } catch {
      return notes;
    }
  }
}
