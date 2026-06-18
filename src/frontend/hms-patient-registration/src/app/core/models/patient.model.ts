export interface PatientSummary {
  id: string;
  patientNumber: string;
  fullName: string;
  dateOfBirth: string;
  gender: string;
  status: string;
  primaryPhone: string | null;
  createdAt: string;
}

export interface PatientAddress {
  addressType: string;
  line1: string;
  line2: string | null;
  city: string;
  state: string | null;
  postalCode: string | null;
  country: string;
  isPrimary: boolean;
}

export interface PatientContact {
  contactType: string;
  value: string;
  isPrimary: boolean;
}

export interface EmergencyContact {
  name: string;
  relationship: string;
  phone: string;
  email: string | null;
  isPrimary: boolean;
}

export interface PatientDetail {
  id: string;
  patientNumber: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
  fullName: string;
  dateOfBirth: string;
  gender: string;
  bloodGroup: string;
  nationalId: string | null;
  nationality: string | null;
  status: string;
  legacyPatientId: string | null;
  notes: string | null;
  createdAt: string;
  updatedAt: string | null;
  addresses: PatientAddress[];
  contacts: PatientContact[];
  insurances: unknown[];
  emergencyContacts: EmergencyContact[];
}

export interface CreatePatientRequest {
  firstName: string;
  middleName?: string | null;
  lastName: string;
  dateOfBirth: string;
  gender: string;
  bloodGroup: string;
  nationalId?: string | null;
  nationality?: string | null;
  notes?: string | null;
  allowDuplicateOverride?: boolean;
  addresses: {
    addressType: string;
    line1: string;
    line2?: string | null;
    city: string;
    state?: string | null;
    postalCode?: string | null;
    country: string;
    isPrimary: boolean;
  }[];
  contacts: {
    contactType: string;
    value: string;
    isPrimary: boolean;
  }[];
  emergencyContacts: {
    name: string;
    relationship: string;
    phone: string;
    email?: string | null;
    isPrimary: boolean;
  }[];
  insurances: unknown[];
}

export interface DashboardStats {
  totalPatients: number;
  newAdmissionsThisMonth: number;
  activeCases: number;
  appointmentsToday: number;
  weeklyActivity: { day: string; patients: number; appointments: number }[];
  departmentDistribution: { name: string; value: number }[];
  recentPatients: PatientSummary[];
}
