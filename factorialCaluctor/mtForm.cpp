#include "mtForm.h"
using namespace System;
using namespace System::windows::forms;
[STAThread]
void main()
{
	Application::EnableVisualStyle();
	Application::SetComptiableTextRenderingfault(false);
	factorialCaluctor::mtform form;
	Application::Run(%form);
}
