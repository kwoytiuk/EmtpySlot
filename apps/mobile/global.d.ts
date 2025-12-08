// Global type augmentations for React Native
declare global {
  var FormData: {
    new(): FormData;
    prototype: FormData;
  };
  var Blob: {
    new(parts?: BlobPart[], options?: BlobPropertyBag): Blob;
    prototype: Blob;
  };
  var File: {
    new(fileBits: BlobPart[], fileName: string, options?: FilePropertyBag): File;
    prototype: File;
  };
  var Buffer: typeof import('buffer').Buffer;
}

export {};
