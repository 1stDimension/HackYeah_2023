export interface IChooseActionProps {
  name: string;
  handleOptionChange: (event: any) => void;
  value: string;
  keys: string[];
  label: string;
}
export interface IChooseFileProps {
  name: string;
  handleFileInput: (file: File) => void;
  value?: File;
  label?: string;
}
export interface IChooseKeyProps {
  name: string;
  handleOptionChange: (event: any) => void;
  value: string;
  keys: string[];
  // inputLabel: string;
  helperLabel?: string;
}
