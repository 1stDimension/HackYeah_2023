export interface IChooseActionProps {
  name: string;
  handleOptionChange: (event: any) => void;
  value: string;
  keys: string[];
  label: string;
  errorMessage?: string;
}
export interface IChooseFileProps {
  name: string;
  handleFileInput: (file: File) => void;
  value?: File;
  label?: string;
  errorMessage?: string;
  fullWidth?: boolean;
}
export interface IChooseKeyProps {
  name: string;
  handleOptionChange: (event: any) => void;
  value: string;
  keys: string[];
  // inputLabel: string;
  helperLabel?: string;
}
